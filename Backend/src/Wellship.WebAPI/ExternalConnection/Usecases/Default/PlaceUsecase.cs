using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2007_会場を登録する
/// </summary>
public class PlaceUsecase : IPlaceUsecase
{
    private readonly List<ErrorObject> _errorObjects;
    private readonly IPlaceRepository _placeRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="placeRepository"></param>
    /// <param name="timeProvider"></param>
    public PlaceUsecase(IPlaceRepository placeRepository, TimeProvider timeProvider)
    {
        _placeRepository = placeRepository;
        _errorObjects = new List<ErrorObject>();
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// EC2007_会場を登録する
    /// </summary>
    /// <param name="places">会場</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StorePlacesAsync(List<Place> places)
    {
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        // WARNING検証
        var warningPlaces = new List<Place>();

        // 必須項目の空値のチェック
        var spaceCheckProperties = new[]
        {
            "Code",     // 会場コード,
            "Name"      // 会場名
        };
        // チェックするプロパティ一覧をメソッドに渡して空値チェックする
        foreach (var warning in ValidationChecker.SpaceCheckProperties(places, spaceCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをPlaceにキャストしてワーニングリストに追加する
            if (warning is Place place)
            {
                warningPlaces.Add(place);
            }
        }

        // キー重複チェック
        var duplicateCheckProperties = new[]
        {
            "Code"      // 会場コード
        };
        // チェックするプロパティ一覧をメソッドに渡して重複チェックする
        foreach (var warning in ValidationChecker.DuplicateCheckProperties(places, duplicateCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをPlaceにキャストしてワーニングリストに追加する
            if (warning is Place place)
            {
                warningPlaces.Add(place);
            }
        }

        // エンティティリスト生成
        var placeEntities = places.Except(warningPlaces)
                                 .Select(item => new PlaceEntity
                                 {
                                     PlaceId = Guid.NewGuid(),
                                     PlaceCode = item.Code,
                                     Name = item.Name
                                 }).ToList();

        // 会場を登録する
        await _placeRepository.UpsertPlacesAsync(placeEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return errorObjects;
    }
}
