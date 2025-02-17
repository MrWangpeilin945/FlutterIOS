using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2007_会場を登録する インターフェース
/// </summary>
public interface IPlaceUsecase
{
    /// <summary>
    /// EC2007_会場を登録する
    /// </summary>
    /// <param name="places">会場</param>
    /// <returns>エラーリスト</returns>
    public Task<List<ErrorObject>> StorePlacesAsync(List<Place> places);
}
