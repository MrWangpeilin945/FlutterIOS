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
        _errorObjects.Clear();

        // Code 必須チェック済みのリストを取得する
        var insertPlacesByRequiredCodes = GetCheckedRequiredCode(places);

        // Code 重複チェック済みのリストを取得する
        var insertPlacesByDuplicateCodes = GetCheckedDuplicateCode(places);

        var commonInsertPlaces = insertPlacesByRequiredCodes.Intersect(insertPlacesByDuplicateCodes)
                                                            .ToList();

        // エンティティリスト生成
        var placeEntities = commonInsertPlaces.Select(item => new PlaceEntity
        {
            PlaceId = Guid.NewGuid(),
            PlaceCode = item.Code,
            Name = item.Name
        }).ToList();

        // 会場を登録する
        await _placeRepository.UpsertPlacesAsync(placeEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return _errorObjects;
    }

    /// <summary>
    /// Code 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="places"></param>
    /// <returns></returns>
    private List<Place> GetCheckedRequiredCode(List<Place> places)
    {
        // WARNING検証
        // キー重複
        var requiredData = places.Where(x => string.IsNullOrWhiteSpace(x.Code));

        if (requiredData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredData);
            return places.Except(requiredData).ToList();
        }
        else
        {
            return new List<Place>(places);
        }
    }

    /// <summary>
    /// Code 重複チェック済みのリストを取得する
    /// </summary>
    /// <param name="places"></param>
    /// <returns></returns>
    private List<Place> GetCheckedDuplicateCode(List<Place> places)
    {
        // WARNING検証
        // キー重複
        var duplicatedPlaceCodes = places.GroupBy(x => x.Code).Where(x => x.Count() > 1).Select(x => x.Key).ToHashSet();

        if (duplicatedPlaceCodes.Any())
        {
            var duplicatedData = places.Where(x => duplicatedPlaceCodes.Contains(x.Code));

            // 返却用エラーオブジェクトに追加
            AddDuplicateDataErrorObjects(duplicatedData);
            return places.Except(duplicatedData).ToList();
        }
        else
        {
            return new List<Place>(places);
        }
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(必須項目エラー）
    /// </summary>
    /// <param name="requiredData"></param>
    private void AddRequiredDataErrorObjects(IEnumerable<Place> requiredData)
    {
        var errorObjects = requiredData
            .Select(r => new ErrorObject
            {
                Code = "10004",
                Message = "必須項目が不足しています。Code",
                InputNote = r.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(キーが重複するレコード）
    /// </summary>
    /// <param name="duplicatedData"></param>
    private void AddDuplicateDataErrorObjects(IEnumerable<Place> duplicatedData)
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
