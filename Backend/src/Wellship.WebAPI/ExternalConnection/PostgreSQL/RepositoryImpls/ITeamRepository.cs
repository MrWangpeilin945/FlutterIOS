using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 班を登録するRepository層
    /// </summary>
    public interface ITeamRepository
    {
        /// <summary>
        /// 班を登録するRepository層
        /// </summary>
        /// <param name="teams">班</param>
        /// <param name="createdAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        public Task UpsertTeamsAsync(List<TeamEntity> teams, DateTimeOffset createdAt, string createdBy);

        /// <summary>
        /// 存在する班コードを取得する。
        /// </summary>
        /// <param name="teamCodes">班コードリスト</param>
        /// <returns>存在する班コードリスト</returns>
        public Task<List<string>> GetTeamsByCodesAsync(List<string> teamCodes);

        /// <summary>
        /// 存在する班情報（ID、コード）を取得する
        /// </summary>
        /// <param name="teamCodes">班コード</param>
        /// <returns></returns>
        public Task<List<TeamEntity>> GetTeamInfoAsync(List<string> teamCodes);
    }

}
