
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// 班を登録するUsecase層
    /// </summary>
    public class TeamUsecases : ITeamUsecases
    {
        private readonly List<ErrorObject> _errorObjects;
        private readonly ITeamRepository _teamRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="teamRepository"></param>
        public TeamUsecases(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
            _errorObjects = new List<ErrorObject>();
        }

        /// <summary>
        /// EC2006_班を登録する
        /// </summary>
        /// <param name="teams">班</param>
        /// <returns>エラーリスト</returns>
        public async Task<List<ErrorObject>> StoreTeamsAsync(List<Team> teams)
        {
            // エンティティリスト生成
            List<TeamEntity> teamEntities = teams.Select(item =>new TeamEntity
            {
                TeamCode = item.Code,
                Name = item.Name
            }).ToList();

            // Repository処理
            await _teamRepository.UpsertTeamsAsync(teamEntities, DateTime.Now, "ExternalConnection");

            return _errorObjects;
        }
    }

}
