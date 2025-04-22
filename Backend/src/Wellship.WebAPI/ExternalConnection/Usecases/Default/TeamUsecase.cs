using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2006_班を登録する
/// </summary>
public class TeamUsecase : ITeamUsecase
{
    private readonly ITeamRepository _teamRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="teamRepository"></param>
    /// <param name="timeProvider"></param>
    public TeamUsecase(ITeamRepository teamRepository, TimeProvider timeProvider)
    {
        _teamRepository = teamRepository;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// EC2006_班を登録する
    /// </summary>
    /// <param name="teams">班</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreTeamsAsync(List<Team> teams)
    {
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        // WARNING検証
        var warningTeams = new List<Team>();

        // 必須項目の空値のチェック
        var spaceCheckProperties = new[]
        {
            "Code",     // 班コード,
            "Name"      // 班名
        };
        // チェックするプロパティ一覧をメソッドに渡して空値チェックする
        foreach (var warning in ValidationChecker.SpaceCheckProperties(teams, spaceCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをTeamにキャストしてワーニングリストに追加する
            warningTeams.Add((Team)warning);
        }

        // キー重複チェック
        var duplicateCheckProperties = new[]
        {
            "Code"      // 班コード
        };
        // チェックするプロパティ一覧をメソッドに渡して重複チェックする
        foreach (var warning in ValidationChecker.DuplicateCheckProperties(teams, duplicateCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをTeamにキャストしてワーニングリストに追加する
            warningTeams.Add((Team)warning);
        }

        // 班エンティティリスト生成
        var teamEntities = teams.Except(warningTeams)
                                .Select(item => new TeamEntity
                                {
                                    TeamId = Guid.NewGuid(),
                                    TeamCode = item.Code,
                                    Name = item.Name
                                }).ToList();

        // 班を登録する
        await _teamRepository.UpsertTeamsAsync(teamEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return errorObjects;
    }
}
