using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2006_班を登録する インターフェース
/// </summary>
public interface ITeamUsecase
{
    /// <summary>
    /// EC2006_班を登録する
    /// </summary>
    /// <param name="teams">班</param>
    /// <returns>エラーリスト</returns>
    public Task<List<ErrorObject>> StoreTeamsAsync(List<Team> teams);
}
