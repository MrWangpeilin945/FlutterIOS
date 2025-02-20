using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2006_班を登録する
/// </summary>
public class TeamUsecase : ITeamUsecase
{
    private readonly List<ErrorObject> _errorObjects;
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
        _errorObjects = new List<ErrorObject>();
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// EC2006_班を登録する
    /// </summary>
    /// <param name="teams">班</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreTeamsAsync(List<Team> teams)
    {
        _errorObjects.Clear();

        // Code 必須チェック済みのリストを取得する
        var insertTeamsByRequiredCodes = GetCheckedRequiredCode(teams);

        // Code 重複チェック済みのリストを取得する
        var insertTeamsByDuplicateCodes = GetCheckedDuplicateCode(teams);

        var commonInsertTeams = insertTeamsByRequiredCodes.Intersect(insertTeamsByDuplicateCodes)
                                                          .ToList();

        // エンティティリスト生成
        var teamEntities = commonInsertTeams.Select(item => new TeamEntity
        {
            TeamId = Guid.NewGuid(),
            TeamCode = item.Code,
            Name = item.Name
        }).ToList();

        // 班を登録する
        await _teamRepository.UpsertTeamsAsync(teamEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return _errorObjects;
    }

    /// <summary>
    /// Code 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="teams"></param>
    /// <returns></returns>
    private List<Team> GetCheckedRequiredCode(List<Team> teams)
    {
        // WARNING検証
        // キー重複
        var requiredData = teams.Where(x => string.IsNullOrWhiteSpace(x.Code));

        if (requiredData.Any())
        {

            // 返却用エラーオブジェクトに追加
            AddRequiredRecordDataErrorObjects(requiredData);
            return teams.Except(requiredData).ToList();
        }
        else
        {
            return new List<Team>(teams);
        }
    }

    /// <summary>
    /// Code 重複チェック済みのリストを取得する
    /// </summary>
    /// <param name="teams"></param>
    /// <returns></returns>
    private List<Team> GetCheckedDuplicateCode(List<Team> teams)
    {
        // WARNING検証
        // キー重複
        var duplicatedTeamCodes = teams.GroupBy(x => x.Code).Where(x => x.Count() > 1).Select(x => x.Key).ToHashSet();

        if (duplicatedTeamCodes.Any())
        {
            var duplicatedData = teams.Where(x => duplicatedTeamCodes.Contains(x.Code));

            // 返却用エラーオブジェクトに追加
            AddDuplicateDataErrorObjects(duplicatedData);
            return teams.Except(duplicatedData).ToList();
        }
        else
        {
            return new List<Team>(teams);
        }
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(必須項目エラー）
    /// </summary>
    /// <param name="requiredData"></param>
    private void AddRequiredRecordDataErrorObjects(IEnumerable<Team> requiredData)
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
    private void AddDuplicateDataErrorObjects(IEnumerable<Team> duplicatedData)
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
