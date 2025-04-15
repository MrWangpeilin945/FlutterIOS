using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2014_基準パターンを登録する
/// </summary>
public class ThresholdUsecase : IThresholdUsecase
{
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
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// EC2014_基準パターンを登録する
    /// </summary>
    /// <param name="thresholds">基準パターン</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreThresholdsAsync(List<Threshold> thresholds)
    {
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        // WARNING検証
        var warningThresholds = new List<Threshold>();

        // 必須項目の空値のチェック
        var spaceCheckProperties = new[]
        {
            "Code",     // 基準値パターンコード,
            "Name"      // 基準値パターン名
        };
        // チェックするプロパティ一覧をメソッドに渡して空値チェックする
        foreach (var warning in ValidationChecker.SpaceCheckProperties(thresholds, spaceCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをThresholdにキャストしてワーニングリストに追加する
            if (warning is Threshold threshold)
            {
                warningThresholds.Add(threshold);
            }
        }

        // キー重複チェック
        var duplicateCheckProperties = new[]
        {
            "Code"      // 基準値パターンコード
        };
        // チェックするプロパティ一覧をメソッドに渡して重複チェックする
        foreach (var warning in ValidationChecker.DuplicateCheckProperties(thresholds, duplicateCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをThresholdにキャストしてワーニングリストに追加する
            if (warning is Threshold threshold)
            {
                warningThresholds.Add(threshold);
            }
        }

        // エンティティリスト生成
        List<ThresholdEntity> thresholdEntities = thresholds.Except(warningThresholds)
                                                            .Select(item => new ThresholdEntity
                                                            {
                                                                ThresholdId = Guid.NewGuid(),
                                                                ThresholdCode = item.Code,
                                                                Name = item.Name
                                                            }).ToList();

        // 基準パターンを登録する
        await _thresholdRepository.UpsertThresholdsAsync(thresholdEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return errorObjects;
    }
}
