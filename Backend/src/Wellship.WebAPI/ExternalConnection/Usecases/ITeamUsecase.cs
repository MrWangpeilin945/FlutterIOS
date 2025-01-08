using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// 班を登録するUsecase層
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
}
