using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2014_基準パターンを登録する
/// </summary>
public class ThresholdUsecase : IThresholdUsecase
{
    private readonly List<ErrorObject> _errorObjects;
    private readonly IThresholdRepository _thresholdRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="thresholdRepository"></param>
    /// <param name="timeProvider"></param>
    public ThresholdUsecase(IThresholdRepository thresholdRepository, TimeProvider timeProvider)
    {
        _thresholdRepository = thresholdRepository;
        _errorObjects = new List<ErrorObject>();
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// EC2014_基準パターンを登録する
    /// </summary>
    /// <param name="thresholds">基準パターン</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreThresholdsAsync(List<Threshold> thresholds)
    {
        _errorObjects.Clear();

        // 必須チェック済みのリストを取得する
        var insertThresholdsByRequireds = GetCheckedRequired(thresholds);

        // 重複チェック済みのリストを取得する
        var insertThresholdsByDuplicateds = GetCheckedDuplicated(thresholds);

        var commonInsertThresholds = insertThresholdsByRequireds.Intersect(insertThresholdsByDuplicateds)
                                                                .ToList();

        // エンティティリスト生成
        List<ThresholdEntity> thresholdEntities = commonInsertThresholds.Select(item => new ThresholdEntity
        {
            ThresholdId = Guid.NewGuid(),
            ThresholdCode = item.Code,
            Name = item.Name
        }).ToList();

        // 基準パターンを登録する
        await _thresholdRepository.UpsertThresholdsAsync(thresholdEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return _errorObjects;
    }

    /// <summary>
    /// 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="thresholds"></param>
    /// <returns></returns>
    private List<Threshold> GetCheckedRequired(List<Threshold> thresholds)
    {
        // WARNING検証
        // 未入力(Code)
        var requiredCodeData = thresholds.Where(x => string.IsNullOrWhiteSpace(x.Code));
        if (requiredCodeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredCodeData, "Code");
        }

        // 未入力(Name)
        var requiredNameData = thresholds.Where(x => string.IsNullOrWhiteSpace(x.Name));
        if (requiredNameData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredNameData, "Name");
        }

        return thresholds.Except(requiredCodeData)
                         .Except(requiredNameData)
                         .ToList();
    }

    /// <summary>
    /// 重複チェック済みのリストを取得する
    /// </summary>
    /// <param name="thresholds"></param>
    /// <returns></returns>
    private List<Threshold> GetCheckedDuplicated(List<Threshold> thresholds)
    {
        // WARNING検証
        // キー重複
        var duplicatedPlaceCodes = thresholds.GroupBy(x => x.Code).Where(x => x.Count() > 1).Select(x => x.Key).ToHashSet();

        if (duplicatedPlaceCodes.Any())
        {
            var duplicatedData = thresholds.Where(x => duplicatedPlaceCodes.Contains(x.Code));

            // 返却用エラーオブジェクトに追加
            AddDuplicateDataErrorObjects(duplicatedData);
            return thresholds.Except(duplicatedData).ToList();
        }
        else
        {
            return new List<Threshold>(thresholds);
        }
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(必須項目エラー）
    /// </summary>
    /// <param name="requiredData"></param>
    /// <param name="itemName"></param>
    private void AddRequiredDataErrorObjects(IEnumerable<Threshold> requiredData, string itemName)
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
    /// エラーオブジェクトに情報追加する(キーが重複するレコード）
    /// </summary>
    /// <param name="duplicatedData"></param>
    private void AddDuplicateDataErrorObjects(IEnumerable<Threshold> duplicatedData)
    {
        var errorObjects = duplicatedData
            .Select(d => new ErrorObject
            {
                Code = "10003",
                Message = $"キー項目が重複しています。Code:{d.Code}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }
}
