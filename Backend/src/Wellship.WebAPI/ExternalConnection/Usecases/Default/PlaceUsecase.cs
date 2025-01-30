using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2007_会場を登録する
/// </summary>
public class PlaceUsecase : IPlaceUsecase
{
    private readonly List<ErrorObject> _errorObjects;
    private readonly IPlaceRepository _placeRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="placeRepository"></param>
    public PlaceUsecase(IPlaceRepository placeRepository)
    {
        _placeRepository = placeRepository;
        _errorObjects = new List<ErrorObject>();
    }

    /// <summary>
    /// EC2007_会場を登録する
    /// </summary>
    /// <param name="places">会場</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StorePlacesAsync(List<Place> places)
    {
        // エンティティリスト生成
        List<PlaceEntity> placeEntities = places.Select(item => new PlaceEntity
        {
            PlaceId = Guid.NewGuid(),
            PlaceCode = item.Code,
            Name = item.Name
        }).ToList();

        // Repository処理
        await _placeRepository.UpsertPlacesAsync(placeEntities, DateTime.Now, "ExternalConnection");

        return _errorObjects;
    }
}
