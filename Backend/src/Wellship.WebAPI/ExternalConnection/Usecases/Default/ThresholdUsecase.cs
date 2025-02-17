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
        // エンティティリスト生成
        List<ThresholdEntity> thresholdEntities = thresholds.Select(item => new ThresholdEntity
        {
            ThresholdId = Guid.NewGuid(),
            ThresholdCode = item.Code,
            Name = item.Name
        }).ToList();

        // Repository処理
        await _thresholdRepository.UpsertThresholdsAsync(thresholdEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return _errorObjects;
    }
}
