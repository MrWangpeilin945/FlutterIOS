using Microsoft.AspNetCore.Mvc;

using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1
{
    /// <summary>
    /// 動作確認用コントローラ
    /// </summary>
    public class TeamTestController : ControllerBase
    {
        private readonly ITeamUsecases _administratorUsecase;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="teamUsecaseUsecase">班ユースケース</param>
        public TeamTestController(ITeamUsecases teamUsecaseUsecase)
        {
            _administratorUsecase = teamUsecaseUsecase;
        }

        /// <summary>
        /// 班登録テスト
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Team))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/team/profile")]
        public async Task<IActionResult> TestTeamAsync()
        {
            // 投入データ作成
            List<Team> teams;
            teams = CreateTestTeams(10000);
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = await _administratorUsecase.StoreTeamsAsync(teams);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
            return Ok(result);
        }

        /// <summary>
        /// 班登録テスト（Upsertテスト）
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Team))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/team_upsert/profile")]
        public async Task<IActionResult> TestTeamUpsertAsync()
        {
            // 投入データ作成
            List<Team> teams;
            teams = CreateUpsertTestTeams();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = await _administratorUsecase.StoreTeamsAsync(teams);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
            return Ok(result);
        }
        
        /// <summary>
        /// 投入データ作成
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        protected List<Team> CreateTestTeams(int num)
        {
            List<Team> teams = new List<Team>();
            for(int i = 1; i < num + 1; i++)
            {
                Team tmp_team = new Team{Code="Team"+i.ToString("00000"), Name="班"+i.ToString("00000"), InputNote=i.ToString() };
                teams.Add((Team)tmp_team);
            }
            return teams;
        }
        
        /// <summary>
        /// 投入データ作成
        /// </summary>
        /// <returns></returns>
        protected List<Team> CreateUpsertTestTeams()
        {
            List<Team> teams = new List<Team>();
            teams.Add(new Team{ Code="Team00010", Name="班upsert00010", InputNote=999.ToString()});
            return teams;
        }
        
        
    }
}
