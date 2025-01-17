using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// EC2009_基準値（範囲）を登録する
    /// </summary>
    public class ExamNormalValueRangeUsecase : IExamNormalValueRangeUsecase
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;
        private readonly List<ErrorObject> _errorObjects;
        private readonly IExamNormalValueRangeRepository _examNormalValueRangeRepository;
        private readonly IThresholdRepository _thresholdRepository;
        private readonly IExternalExamItemDetailsRepository _externalExamItemDetailsRepository;

        /// <summary>
        /// ユースケースを作成する
        /// </summary>
        /// <param name="dbConnectionProvider"></param>
        /// <param name="examNormalValueRangeRepository"></param>
        public ExamNormalValueRangeUsecase(IDbConnectionProvider dbConnectionProvider, IExamNormalValueRangeRepository examNormalValueRangeRepository, IThresholdRepository thresholdRepository, IExternalExamItemDetailsRepository externalExamItemDetailsRepository)
        {
            _dbConnectionProvider = dbConnectionProvider;
            _errorObjects = new List<ErrorObject>();
            _examNormalValueRangeRepository = examNormalValueRangeRepository;
            _thresholdRepository = thresholdRepository;
            _externalExamItemDetailsRepository = externalExamItemDetailsRepository;
        }

        /// <summary>
        /// 基準値（範囲）を登録する
        /// </summary>
        /// <param name="examNormalValueRanges"></param>
        /// <returns></returns>
        public async Task<List<ErrorObject>> StoreExamNormalValueRangeAsync(List<ExamNormalValueRange> examNormalValueRanges)
        {
            // 既存基準値パターンコードの確認
            var insertExamNormalValueRangesByThresholdCodes = await GetCheckedThresholdCodes(examNormalValueRanges);

            // 検査項目明細IDの確認
            var insertExamNormalValueRangesByExamItemDetails = await GetCheckedExamItemDetails(examNormalValueRanges);

            // 基準値パターンIDの取得
            var thresholds = await _thresholdRepository.GetThresholdsByCodesAsync(examNormalValueRanges.Select(c => c.ThresholdCd).ToList());

            // 検査項目明細IDの取得
            var externalExamItemDetails = await _externalExamItemDetailsRepository.GetDetailsByCodesAsync(examNormalValueRanges.Select(c => c.ExamItemDetailCd).ToList());

            var commonInsertExamNormalValueRanges = insertExamNormalValueRangesByThresholdCodes
                .Intersect(insertExamNormalValueRangesByExamItemDetails)
                .ToList();

            // 主キーの重複確認
            var insertExamNormalValueRanges = await CheckDuplicateExamNormalValueRanges(commonInsertExamNormalValueRanges, thresholds, externalExamItemDetails);


            // 基準値範囲エンティティリストを生成
            var examNormalValueRangeEntities = insertExamNormalValueRanges.Select(examNormalValueRange => new ExamNormalValueRangeEntity
            {
                Name = examNormalValueRange.Name,
                ThresholdId = thresholds.Where(t => t.ThresholdCode == examNormalValueRange.ThresholdCd).Select(t => t.ThresholdId).FirstOrDefault(),
                ExamItemDetailId = externalExamItemDetails.Where(e => e.ExternalExamItemDetailCode == examNormalValueRange.ExamItemDetailCd).Select(e => e.ExamItemDetailId).FirstOrDefault(),
                MinAge = examNormalValueRange.MinAge.ToString().PadLeft(7, '0'),
                MaxAge = examNormalValueRange.MaxAge.ToString().PadLeft(7, '0'),
                TargetSex = (int)examNormalValueRange.TargetSex,
                MinValue = examNormalValueRange.MinValue,
                MaxValue = examNormalValueRange.MaxValue,
                ErrorLevel = (int)examNormalValueRange.ErrorLevel
            }).ToList();

            DateTime createdAt = DateTime.Now;
            string createdBy = "ExternalConnection";

            // 基準値範囲を登録する
            await _examNormalValueRangeRepository.UpsertExamNormalValueRangeAsync(examNormalValueRangeEntities, createdAt, createdBy);

            return _errorObjects;
        }

        /// <summary>
        /// 既存基準値パターンコードの確認
        /// </summary>
        /// <param name="examNormalValueRanges"></param>
        /// <returns></returns>
        private async Task<List<ExamNormalValueRange>> GetCheckedThresholdCodes(List<ExamNormalValueRange> examNormalValueRanges)
        {
            var results = new List<ExamNormalValueRange>();

            // WARNING検証
            // 基準値パターンコード
            var errorThresholdCodes = await ThresholdCodesExists(examNormalValueRanges);

            if (errorThresholdCodes.Count > 0)
            {
                // 返却用エラーオブジェクトに追加
                AddThresholdCodeErrorObjects(examNormalValueRanges, errorThresholdCodes);
                // 会場コードが存在する会場日程のみ抽出
                results = examNormalValueRanges
                    .Where(examNormalValueRange => !errorThresholdCodes.Contains(examNormalValueRange.ThresholdCd))
                    .ToList();
            }
            else
            {
                results = new List<ExamNormalValueRange>(examNormalValueRanges);
            }
            return results;
        }

        /// <summary>
        /// 既存外部コード検査項目明細コードの確認
        /// </summary>
        /// <param name="examNormalValueRanges"></param>
        /// <returns></returns>
        private async Task<List<ExamNormalValueRange>> GetCheckedExamItemDetails(List<ExamNormalValueRange> examNormalValueRanges)
        {
            var results = new List<ExamNormalValueRange>();

            // WARNING検証
            // 外部コード検査項目明細コード
            var errorExternalExamItemDetailCodes = await ExternalExamItemDetailCodesExists(examNormalValueRanges);

            if (errorExternalExamItemDetailCodes.Count > 0)
            {
                // 返却用エラーオブジェクトに追加
                AddExternalExamItemDetailCodesErrorObjects(examNormalValueRanges, errorExternalExamItemDetailCodes);
                // 会場コードが存在する会場日程のみ抽出
                results = examNormalValueRanges
                    .Where(examNormalValueRange => !errorExternalExamItemDetailCodes.Contains(examNormalValueRange.ExamItemDetailCd))
                    .ToList();
            }
            else
            {
                results = new List<ExamNormalValueRange>(examNormalValueRanges);
            }
            return results;
        }

        /// <summary>
        /// PKが重複するレコードの確認
        /// </summary>
        /// <param name="examNormalValueRanges"></param>
        /// <returns></returns>
        private async Task<List<ExamNormalValueRange>> CheckDuplicateExamNormalValueRanges(List<ExamNormalValueRange> examNormalValueRanges, List<ThresholdEntity> thresholds, List<ExternalExamItemDetailEntity> externalExamItemDetails)
        {
            var results = new List<ExamNormalValueRange>();

            var duplicatedData = await GetDuplicatedData(examNormalValueRanges, thresholds, externalExamItemDetails);

            if (duplicatedData.Count > 0)
            {
                // 返却用エラーオブジェクトに追加
                AddDuplicateDataErrorObjects(duplicatedData);
                results = examNormalValueRanges.Except(duplicatedData).ToList();
            }
            else
            {
                results = new List<ExamNormalValueRange>(examNormalValueRanges);
            }
            return results;
        }

        /// <summary>
        /// 基準値コードの取得
        /// </summary>
        /// <param name="examNormalValueRanges"></param>
        /// <returns></returns>
        private async Task<List<string>> ThresholdCodesExists(List<ExamNormalValueRange> examNormalValueRanges)
        {
            var results = new List<string>();

            // 基準値コードを取得する
            var thresholdCodes = examNormalValueRanges
                .Select(examNormalValueRange => examNormalValueRange.ThresholdCd)
                .Distinct()
                .ToList();

            // 基準値コードを基に基準値パターンを取得する
            var existThresholds = await _thresholdRepository.GetThresholdsByCodesAsync(thresholdCodes);
            var existThresholdCodes = existThresholds.Select(t => t.ThresholdCode).ToList();
            // 存在しない基準値コードを取得する
            results = thresholdCodes.Except(existThresholdCodes).ToList();

            return results;
        }

        /// <summary>
        /// 外部コード検査項目明細コードの取得
        /// </summary>
        /// <param name="examNormalValueRanges"></param>
        /// <returns></returns>
        private async Task<List<string>> ExternalExamItemDetailCodesExists(List<ExamNormalValueRange> examNormalValueRanges)
        {
            var results = new List<string>();

            // 外部コード検査項目明細コードを取得する
            var externalExamItemDetailCodes = examNormalValueRanges
                .Select(examNormalValueRange => examNormalValueRange.ExamItemDetailCd)
                .Distinct()
                .ToList();

            // 外部コード検査項目明細コードを基に外部検査項目明細を取得する
            var existsExternalExamItemDetails = await _externalExamItemDetailsRepository.GetDetailsByCodesAsync(externalExamItemDetailCodes);
            var existsExternalExamItemDetailCodes = existsExternalExamItemDetails.Select(e => e.ExternalExamItemDetailCode).ToList();

            // 存在しない基準値コードを取得する
            results = externalExamItemDetailCodes.Except(existsExternalExamItemDetailCodes).ToList();

            return results;
        }

        /// <summary>
        /// 重複チェック
        /// </summary>
        /// <param name="examNormalValueRanges"></param>
        /// <param name="thresholds"></param>
        /// <param name="externalExamItemDetails"></param>
        /// <returns></returns>
        private async Task<List<ExamNormalValueRange>> GetDuplicatedData(List<ExamNormalValueRange> examNormalValueRanges, List<ThresholdEntity> thresholds, List<ExternalExamItemDetailEntity> externalExamItemDetails)
        {
            // ThresholdCode と ThresholdId のマッピングを作成
            var thresholdMap = thresholds.ToDictionary(t => t.ThresholdCode, t => t.ThresholdId);

            // ExternalExamItemDetailCode と ExamItemDetailId のマッピングを作成
            var examItemDetailMap = externalExamItemDetails.ToDictionary(e => e.ExternalExamItemDetailCode, e => e.ExamItemDetailId);

            // 重複データを検索
            var duplicateDataKeys = examNormalValueRanges
                .GroupBy(range => new
                {
                    ThresholdId = thresholdMap.TryGetValue(range.ThresholdCd, out var thresholdId) ? thresholdId : Guid.Empty,
                    ExamItemDetailId = examItemDetailMap.TryGetValue(range.ExamItemDetailCd, out var examItemDetailId) ? examItemDetailId : 0,
                    range.TargetSex,
                    MaxAge = range.MaxAge.PadLeft(7, '0'), // MaxAgeを左ゼロ埋めで7桁に整形
                    range.MaxValue
                })
                .Where(g => g.Count() > 1) // 重複しているグループのみ
                .Select(g => g.Key) // 重複の条件キーを取得
                .ToList();

            // 重複条件に一致するExamNormalValueRangeを抽出
            var duplicateData = examNormalValueRanges
                .Where(range => duplicateDataKeys.Any(key =>
                    thresholdMap.TryGetValue(range.ThresholdCd, out var thresholdId) && thresholdId == key.ThresholdId &&
                    examItemDetailMap.TryGetValue(range.ExamItemDetailCd, out var examItemDetailId) && examItemDetailId == key.ExamItemDetailId &&
                    range.TargetSex == key.TargetSex &&
                    range.MaxAge.PadLeft(7, '0') == key.MaxAge &&
                    range.MaxValue == key.MaxValue))
                .ToList();

            return duplicateData;
        }
 
        /// <summary>
        /// エラーオブジェクトに情報追加する(基準値コード）
        /// </summary>
        /// <param name="examNormalValueRanges">基準値範囲</param>
        /// <param name="errorThresholdCodes">取得に失敗した基準値コードリスト</param>
        private void AddThresholdCodeErrorObjects(IEnumerable<ExamNormalValueRange> examNormalValueRanges, List<string> errorThresholdCodes)
        {
            // 取得できないエラーを返却用エラーオブジェクトに追加
            var errorObjects = examNormalValueRanges
                .Where(examNormalValueRange => errorThresholdCodes.Contains(examNormalValueRange.ThresholdCd))
                .Select(examNormalValueRange => new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたThresholdCdがシステム上に存在しません。Code:{examNormalValueRange.ThresholdCd}",
                    InputNote = examNormalValueRange.InputNote
                }).ToList();

            _errorObjects.AddRange(errorObjects);
        }

        /// <summary>
        /// エラーオブジェクトに情報追加する(外部コード検査項目明細コード）
        /// </summary>
        /// <param name="examNormalValueRanges">基準値範囲</param>
        /// <param name="errorExternalExamItemDetailCodes">取得に失敗した外部コード検査項目明細コードリスト</param>
        private void AddExternalExamItemDetailCodesErrorObjects(IEnumerable<ExamNormalValueRange> examNormalValueRanges, List<string> errorExternalExamItemDetailCodes)
        {
            // 取得できないエラーを返却用エラーオブジェクトに追加
            var errorObjects = examNormalValueRanges
                .Where(examNormalValueRange => errorExternalExamItemDetailCodes.Contains(examNormalValueRange.ExamItemDetailCd))
                .Select(examNormalValueRange => new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたExamItemDetailCdがシステム上に存在しません。Code:{examNormalValueRange.ExamItemDetailCd}",
                    InputNote = examNormalValueRange.InputNote
                }).ToList();

            _errorObjects.AddRange(errorObjects);
        }

        /// <summary>
        /// エラーオブジェクトに情報追加する(PKが重複するレコード）
        /// </summary>
        /// <param name="duplicatedData"></param>
        private void AddDuplicateDataErrorObjects(List<ExamNormalValueRange> duplicatedData)
        {
            var errorObjects = duplicatedData
                .Select(d => new ErrorObject
                {
                    Code = "10003",
                    Message = $"キー項目が重複しています。Code:{d.ThresholdCd}/{d.ExamItemDetailCd}/{d.TargetSex}/{d.MaxAge}/{d.MaxValue}",
                    InputNote = d.InputNote
                }).ToList();

            _errorObjects.AddRange(errorObjects);
        }
    }
}
