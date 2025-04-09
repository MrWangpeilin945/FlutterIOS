using System.Text.RegularExpressions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2009_基準値（範囲）を登録する
/// </summary>
public class ExamNormalValueRangeUsecase : IExamNormalValueRangeUsecase
{
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
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        // 基準値パターンIDの取得
        var thresholds = await _thresholdRepository.GetThresholdsByCodesAsync(examNormalValueRanges.Select(c => c.ThresholdCode).Distinct().ToList());

        // 検査項目明細IDの取得
        var externalExamItemDetails = await _externalExamItemDetailsRepository.GetDetailsByCodesAsync(examNormalValueRanges.Select(c => c.ExamItemDetailCode).Distinct().ToList());

        // WARNING検証
        var warningExamNormalValueRanges = new List<ExamNormalValueRange>();

        // 必須項目の空値のチェック
        var spaceCheckProperties = new[]
        {
            "Name",                 // 名称
            "ThresholdCode",        // 基準値パターンCD
            "ExamItemDetailCode",   // 検査項目明細CD
            "MaxAge",               // 対象年齢上限
            "MinAge"                // 対象年齢下限
        };
        // チェックするプロパティ一覧をメソッドに渡してチェックエラーのconsultを取得する
        foreach (var warning in ValidationChecker.SpaceCheckProperties(examNormalValueRanges, spaceCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをexamNormalValueRangeにキャストしてワーニングリストに追加する
            if (warning is ExamNormalValueRange examNormalValueRange)
            {
                warningExamNormalValueRanges.Add(examNormalValueRange);
            }
        }

        // 年齢の形式確認
        var stringPatternCheckProperties = new[]
        {
            "MaxAge",           // 対象年齢上限
            "MinAge"            // 対象年齢下限
        };
        var patterns = new [] { @"^[0-9]{3}(0[0-9]|1[01])([012][0-9]|30)$", @"^[0-9]{3}(0[0-9]|1[01])([012][0-9]|30)$" };
        foreach (var warning in ValidationChecker.StringPatternCheckProperties(examNormalValueRanges, stringPatternCheckProperties, patterns, errorObjects))
        {
            // エラーのオブジェクトをexamNormalValueRangeにキャストしてワーニングリストに追加する
            if (warning is ExamNormalValueRange examNormalValueRange)
            {
                warningExamNormalValueRanges.Add(examNormalValueRange);
            }
        }

        // 年齢の大小確認
        foreach (var warning in examNormalValueRanges.Where(x => x.MinAge.PadLeft(7, '0').CompareTo(x.MaxAge.PadLeft(7, '0')) >= 0))
        {
            warningExamNormalValueRanges.Add(warning);
            errorObjects.Add(new ErrorObject
            {
                Code = "10007",
                Message = $"値の範囲が無効です。MinAge:{warning.MinAge}/MaxAge:{warning.MaxAge}",
                InputNote = warning.InputNote
            });
        }

        // 基準値の大小確認
        foreach (var warning in examNormalValueRanges.Where(x => x.MinValue >= x.MaxValue))
        {
            warningExamNormalValueRanges.Add(warning);
            errorObjects.Add(new ErrorObject
            {
                Code = "10007",
                Message = $"値の範囲が無効です。MinValue:{warning.MinValue}/MaxValue:{warning.MaxValue}",
                InputNote = warning.InputNote
            });
        }

        // 基準値パターンの確認
        foreach (var warning in examNormalValueRanges.Where(x => !thresholds.Select(t => t.ThresholdCode).Contains(x.ThresholdCode)))
        {
            warningExamNormalValueRanges.Add(warning);
            errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたThresholdCodeがシステム上に存在しません。Code:{warning.ThresholdCode}",
                InputNote = warning.InputNote
            });
        }

        // 検査項目明細IDの確認
        foreach (var warning in examNormalValueRanges.Where(x => !externalExamItemDetails.Select(e => e.ExternalExamItemDetailCode).Contains(x.ExamItemDetailCode)))
        {
            warningExamNormalValueRanges.Add(warning);
            errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたExamItemDetailCodeがシステム上に存在しません。Code:{warning.ExamItemDetailCode}",
                InputNote = warning.InputNote
            });
        }

        // PKが重複するレコードの確認
        // 外部コード検査項目明細CDと検査項目明細IDを連結する
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
        // 基準値パターンCD/検査項目明細ID/対象性別/対象年齢上限/値上限の重複キーを取得する
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
            // PKが重複したレコードを取得する
            var duplicatedData = examNormalValueRanges.Where(x => 
                                                        duplicateKeys.Any(duplicateKey =>
                                                            duplicateKey.ExamItemDetailCode == x.ExamItemDetailCode && 
                                                            duplicateKey.ThresholdCode == x.ThresholdCode &&
                                                            duplicateKey.TargetSex == x.TargetSex &&
                                                            duplicateKey.MaxAge == x.MaxAge.ToString().PadLeft(7, '0') && 
                                                            duplicateKey.MaxValue == x.MaxValue
                                                        )); 
            foreach (var warning in duplicatedData)
            {
                warningExamNormalValueRanges.Add(warning);
                errorObjects.Add(new ErrorObject
                {
                    Code = "10003",
                    Message = $"キー項目が重複しています。ThresholdCode:{warning.ThresholdCode}/ExamItemDetailCode:{warning.ExamItemDetailCode}/TargetSex:{warning.TargetSex}/MaxAge:{warning.MaxAge}/MaxValue:{warning.MaxValue}",
                    InputNote = warning.InputNote
                });
            }                                                   
        }
        // 基準値範囲エンティティリストを生成
        var examNormalValueRangeEntities = examNormalValueRanges.Except(warningExamNormalValueRanges)
                                                                .Select(examNormalValueRange => new ExamNormalValueRangeEntity
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

        return errorObjects;
    }
}
