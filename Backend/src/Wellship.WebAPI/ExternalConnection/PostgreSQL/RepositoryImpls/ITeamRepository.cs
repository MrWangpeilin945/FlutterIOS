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
        public Task UpsertTeamsAsync(List<TeamEntity> teams, DateTime createdAt, string createdBy);

    }
}
