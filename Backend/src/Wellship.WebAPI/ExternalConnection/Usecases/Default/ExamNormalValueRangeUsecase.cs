using System.Text.RegularExpressions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2009_基準値（範囲）を登録する
/// </summary>
public class ExamNormalValueRangeUsecase : IExamNormalValueRangeUsecase
{
    private readonly List<ErrorObject> _errorObjects;
    private readonly IExamNormalValueRangeRepository _examNormalValueRangeRepository;
    private readonly IThresholdRepository _thresholdRepository;
    private readonly IExternalExamItemDetailsRepository _externalExamItemDetailsRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// ユースケースを作成する
    /// </summary>
    /// <param name="examNormalValueRangeRepository"></param>
    /// <param name="thresholdRepository"></param>
    /// <param name="externalExamItemDetailsRepository"></param>
    /// <param name="timeProvider"></param>
    public ExamNormalValueRangeUsecase(IExamNormalValueRangeRepository examNormalValueRangeRepository,
                                       IThresholdRepository thresholdRepository, IExternalExamItemDetailsRepository externalExamItemDetailsRepository,
                                       TimeProvider timeProvider)
    {
        _errorObjects = new List<ErrorObject>();
        _examNormalValueRangeRepository = examNormalValueRangeRepository;
        _thresholdRepository = thresholdRepository;
        _externalExamItemDetailsRepository = externalExamItemDetailsRepository;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 基準値（範囲）を登録する
    /// </summary>
    /// <param name="examNormalValueRanges"></param>
    /// <returns></returns>
    public async Task<List<ErrorObject>> StoreExamNormalValueRangeAsync(List<ExamNormalValueRange> examNormalValueRanges)
    {
        _errorObjects.Clear();

        // 必須チェック済みのリストを取得する
        var insertExamNormalValueRangeByRequired = GetCheckedRequired(examNormalValueRanges);

        // 基準値パターンコードの確認
        var insertExamNormalValueRangesByThresholdCodes = await GetCheckedThresholdCodes(examNormalValueRanges);

        // 検査項目明細IDの確認
        var insertExamNormalValueRangesByExamItemDetails = await GetCheckedExamItemDetails(examNormalValueRanges);

        // 年齢の形式確認
        var insertExamNormalValueRangesByAgeInvalid = GetCheckedAgeInvalid(examNormalValueRanges);

        // 値の大小確認
        var insertExamNormalValueRangeByCompared = GetCheckedCompareValue(examNormalValueRanges);


        // 基準値パターンIDの取得
        var thresholds = await _thresholdRepository.GetThresholdsByCodesAsync(examNormalValueRanges.Select(c => c.ThresholdCode).ToList());

        // 検査項目明細IDの取得
        var externalExamItemDetails = await _externalExamItemDetailsRepository.GetDetailsByCodesAsync(examNormalValueRanges.Select(c => c.ExamItemDetailCode).ToList());


        // キー重複の確認
        // 外部コード検査項目明細コード ※ユニットテストコード後、書き直す
        var insertExamNormalValueRangeByDuplicated = new List<ExamNormalValueRange>(examNormalValueRanges);
        var examItemDetail = from normalValueRange in examNormalValueRanges
                             join externalExamItemDetail in externalExamItemDetails
                             on normalValueRange.ExamItemDetailCode equals externalExamItemDetail.ExternalExamItemDetailCode
                             select new
                             {
                                 ExamItemDetailID = externalExamItemDetail.ExamItemDetailId,
                                 normalValueRange.ThresholdCode,
                                 normalValueRange.ExamItemDetailCode,
                                 normalValueRange.TargetSex,
                                 MaxAge = normalValueRange.MaxAge.PadLeft(7, '0'),
                                 normalValueRange.MaxValue
                             };
        var duplicateKeys = examItemDetail.GroupBy(x => new { x.ThresholdCode, x.ExamItemDetailID, x.TargetSex, x.MaxAge, x.MaxValue })
                                          .Where(x => x.Count() > 1)
                                          .SelectMany(x => x.Select(y => new
                                          {
                                              y.ThresholdCode,
                                              y.ExamItemDetailID,
                                              y.TargetSex,
                                              y.MaxAge,
                                              y.MaxValue,
                                              y.ExamItemDetailCode
                                          })).ToList();
        if (duplicateKeys.Any())
        {
            var duplicatedData = examNormalValueRanges.Where(x => 
                                                        duplicateKeys.Any(duplicateKey =>
                                                            duplicateKey.ExamItemDetailCode == x.ExamItemDetailCode && 
                                                            duplicateKey.ThresholdCode == x.ThresholdCode &&
                                                            duplicateKey.TargetSex == x.TargetSex &&
                                                            duplicateKey.MaxAge == x.MaxAge.ToString().PadLeft(7, '0') && 
                                                            duplicateKey.MaxValue == x.MaxValue
                                                        ));            
            // 返却用エラーオブジェクトに追加
            AddDuplicateErrorObjects(duplicatedData.ToList());
            insertExamNormalValueRangeByDuplicated = examNormalValueRanges.Except(duplicatedData).ToList();
        }

        var commonInsertExamNormalValueRanges = insertExamNormalValueRangeByRequired.Intersect(insertExamNormalValueRangesByThresholdCodes)
                                                                       .Intersect(insertExamNormalValueRangesByExamItemDetails)
                                                                       .Intersect(insertExamNormalValueRangeByDuplicated)
                                                                       .Intersect(insertExamNormalValueRangesByAgeInvalid)
                                                                       .Intersect(insertExamNormalValueRangeByCompared)
                                                                       .ToList();


        // 基準値範囲エンティティリストを生成
        var examNormalValueRangeEntities = commonInsertExamNormalValueRanges.Select(examNormalValueRange => new ExamNormalValueRangeEntity
        {
            Name = examNormalValueRange.Name,
            ThresholdId = thresholds.Where(t => t.ThresholdCode == examNormalValueRange.ThresholdCode).Select(t => t.ThresholdId).FirstOrDefault(),
            ExamItemDetailId = externalExamItemDetails.Where(e => e.ExternalExamItemDetailCode == examNormalValueRange.ExamItemDetailCode).Select(e => e.ExamItemDetailId).FirstOrDefault(),
            MinAge = examNormalValueRange.MinAge.ToString().PadLeft(7, '0'),
            MaxAge = examNormalValueRange.MaxAge.ToString().PadLeft(7, '0'),
            TargetSex = (int)examNormalValueRange.TargetSex,
            MinValue = examNormalValueRange.MinValue,
            MaxValue = examNormalValueRange.MaxValue,
            ErrorLevel = (int)examNormalValueRange.ErrorLevel
        }).ToList();

        var createdAt = _timeProvider.GetUtcNow();
        string createdBy = "ExternalConnection";

        // 基準値範囲を登録する
        await _examNormalValueRangeRepository.UpsertExamNormalValueRangeAsync(examNormalValueRangeEntities, createdAt, createdBy);

        return _errorObjects;
    }

    /// <summary>
    /// 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="examNormalValueRanges"></param>
    /// <returns></returns>
    private List<ExamNormalValueRange> GetCheckedRequired(List<ExamNormalValueRange> examNormalValueRanges)
    {
        // WARNING検証
        // 未入力
        var requiredNameData = examNormalValueRanges.Where(x => string.IsNullOrWhiteSpace(x.Name));
        if (requiredNameData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredNameData, "Name");
        }

        // 未入力
        var requiredThresholdCdData = examNormalValueRanges.Where(x => string.IsNullOrWhiteSpace(x.ThresholdCode));
        if (requiredThresholdCdData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredThresholdCdData, "ThresholdCode");
        }

        // 未入力
        var requiredExamItemDetailCdData = examNormalValueRanges.Where(x => string.IsNullOrWhiteSpace(x.ExamItemDetailCode));
        if (requiredExamItemDetailCdData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredExamItemDetailCdData, "ExamItemDetailCode");
        }

        // 未入力
        var requiredMaxAgeData = examNormalValueRanges.Where(x => string.IsNullOrWhiteSpace(x.MaxAge));
        if (requiredMaxAgeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredMaxAgeData, "MaxAge");
        }

        // 未入力
        var requiredMinAgeData = examNormalValueRanges.Where(x => string.IsNullOrWhiteSpace(x.MinAge));
        if (requiredMinAgeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredMinAgeData, "MinAge");
        }

        return examNormalValueRanges.Except(requiredNameData)
                                    .Except(requiredThresholdCdData)
                                    .Except(requiredExamItemDetailCdData)
                                    .Except(requiredMaxAgeData)
                                    .ToList();
    }

    /// <summary>
    /// キー重複の確認
    /// </summary>
    /// <param name="examNormalValueRanges"></param>
    /// <returns></returns>
    private List<ExamNormalValueRange> GetCheckedDuplicateKey(List<ExamNormalValueRange> examNormalValueRanges)
    {
        // キー重複
        var duplicateKeys = examNormalValueRanges.GroupBy(x => new { x.ThresholdCode, x.ExamItemDetailCode, x.TargetSex, MaxAge = x.MaxAge.PadLeft(7, '0'), x.MaxValue })
                                                 .Where(x => x.Count() > 1)
                                                 .Select(x => x.Key).ToHashSet();
        if (duplicateKeys.Any())
        {
            var duplicatedData = examNormalValueRanges.Where(x => duplicateKeys.Contains(new { x.ThresholdCode, x.ExamItemDetailCode, x.TargetSex, MaxAge = x.MaxAge.PadLeft(7, '0'), x.MaxValue }));
            // 返却用エラーオブジェクトに追加
            AddDuplicateErrorObjects(duplicatedData.ToList());
            return examNormalValueRanges.Except(duplicatedData).ToList();
        }
        else
        {
            return new List<ExamNormalValueRange>(examNormalValueRanges);
        }
    }

    /// <summary>
    /// 年齢の形式確認
    /// </summary>
    /// <param name="examNormalValueRanges"></param>
    /// <returns></returns>
    private List<ExamNormalValueRange> GetCheckedAgeInvalid(List<ExamNormalValueRange> examNormalValueRanges)
    {
        string pattern = @"^[0-9]{3}(0[0-9]|1[01])([012][0-9]|30)$";

        var invalidMaxAgeData = examNormalValueRanges.Where(x => !Regex.IsMatch(x.MaxAge, pattern));
        if (invalidMaxAgeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddMaxAgeErrorObjects(invalidMaxAgeData.ToList());
        }

        var invalidMinAgeData = examNormalValueRanges.Where(x => !Regex.IsMatch(x.MinAge, pattern));
        if (invalidMinAgeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddMinAgeErrorObjects(invalidMinAgeData.ToList());
        }

        return examNormalValueRanges.Except(invalidMaxAgeData)
                                    .Except(invalidMinAgeData)
                                    .ToList();
    }

    /// <summary>
    /// 上下限の大小比較の確認
    /// </summary>
    /// <param name="examNormalValueRanges"></param>
    /// <returns></returns>
    private List<ExamNormalValueRange> GetCheckedCompareValue(List<ExamNormalValueRange> examNormalValueRanges)
    {
        // 年齢大小
        var compareErrorAgeData = examNormalValueRanges.Where(x => x.MinAge.PadLeft(7, '0').CompareTo(x.MaxAge.PadLeft(7, '0')) >= 0);
        if (compareErrorAgeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddAgeCompareErrorObjects(compareErrorAgeData.ToList());
        }

        // 基準値大小
        var compareErrorValueData = examNormalValueRanges.Where(x => x.MinValue >= x.MaxValue);
        if (compareErrorValueData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddValueCompareErrorObjects(compareErrorValueData.ToList());
        }

        return examNormalValueRanges.Except(compareErrorAgeData)
                                    .Except(compareErrorValueData)
                                    .ToList();
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
            // 基準値パターンが存在する基準値範囲のみ抽出
            results = examNormalValueRanges
                .Where(examNormalValueRange => !errorThresholdCodes.Contains(examNormalValueRange.ThresholdCode))
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
            // 検査項目明細が存在する基準値範囲のみ抽出
            results = examNormalValueRanges
                .Where(examNormalValueRange => !errorExternalExamItemDetailCodes.Contains(examNormalValueRange.ExamItemDetailCode))
                .ToList();
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
            .Select(examNormalValueRange => examNormalValueRange.ThresholdCode)
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
            .Select(examNormalValueRange => examNormalValueRange.ExamItemDetailCode)
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
    /// エラーオブジェクトに情報追加する(必須項目エラー）
    /// </summary>
    /// <param name="requiredData"></param>
    /// <param name="itemName"></param>
    private void AddRequiredDataErrorObjects(IEnumerable<ExamNormalValueRange> requiredData, string itemName)
    {
        var errorObjects = requiredData
            .Select(r => new ErrorObject
            {
                Code = "10004",
                Message = $"必須項目が不足しています。{itemName}",
                InputNote = r.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
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
            .Where(examNormalValueRange => errorThresholdCodes.Contains(examNormalValueRange.ThresholdCode))
            .Select(examNormalValueRange => new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたThresholdCodeがシステム上に存在しません。Code:{examNormalValueRange.ThresholdCode}",
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
            .Where(examNormalValueRange => errorExternalExamItemDetailCodes.Contains(examNormalValueRange.ExamItemDetailCode))
            .Select(examNormalValueRange => new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたExamItemDetailCodeがシステム上に存在しません。Code:{examNormalValueRange.ExamItemDetailCode}",
                InputNote = examNormalValueRange.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(PKが重複するレコード）
    /// </summary>
    /// <param name="duplicatedData"></param>
    private void AddDuplicateErrorObjects(List<ExamNormalValueRange> duplicatedData)
    {
        var errorObjects = duplicatedData
            .Select(d => new ErrorObject
            {
                Code = "10003",
                Message = $"キー項目が重複しています。ThresholdCode:{d.ThresholdCode}/ExamItemDetailCode:{d.ExamItemDetailCode}/TargetSex:{d.TargetSex}/MaxAge:{d.MaxAge}/MaxValue:{d.MaxValue}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(年齢上限の形式が無効）
    /// </summary>
    /// <param name="examNormalValueRanges"></param>
    private void AddMaxAgeErrorObjects(List<ExamNormalValueRange> examNormalValueRanges)
    {
        var errorObjects = examNormalValueRanges
            .Select(d => new ErrorObject
            {
                Code = "10006",
                Message = $"値の形式が無効です。MaxAge:{d.MaxAge}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(年齢下限の形式が無効）
    /// </summary>
    /// <param name="examNormalValueRanges"></param>
    private void AddMinAgeErrorObjects(List<ExamNormalValueRange> examNormalValueRanges)
    {
        var errorObjects = examNormalValueRanges
            .Select(d => new ErrorObject
            {
                Code = "10006",
                Message = $"値の形式が無効です。MinAge:{d.MinAge}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(年齢大小）
    /// </summary>
    /// <param name="examNormalValueRanges"></param>
    private void AddAgeCompareErrorObjects(List<ExamNormalValueRange> examNormalValueRanges)
    {
        var errorObjects = examNormalValueRanges
            .Select(d => new ErrorObject
            {
                Code = "10007",
                Message = $"値の範囲が無効です。MinAge:{d.MinAge}/MaxAge:{d.MaxAge}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(基準値大小）
    /// </summary>
    /// <param name="examNormalValueRanges"></param>
    private void AddValueCompareErrorObjects(List<ExamNormalValueRange> examNormalValueRanges)
    {
        var errorObjects = examNormalValueRanges
            .Select(d => new ErrorObject
            {
                Code = "10007",
                Message = $"値の範囲が無効です。MinValue:{d.MinValue}/MaxValue:{d.MaxValue}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

}
