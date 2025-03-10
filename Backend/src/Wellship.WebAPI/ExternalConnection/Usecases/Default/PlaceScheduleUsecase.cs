using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.Core.Enums;
using PlaceSchedule = Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard.PlaceSchedule;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2012_会場日程を登録する
/// </summary>
public class PlaceScheduleUsecase : IPlaceScheduleUsecase
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly List<ErrorObject> _errorObjects;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;
    private readonly IPlaceRepository _placeRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// ユースケースを作成する
    /// </summary>
    /// <param name="dbConnectionProvider"></param>
    /// <param name="placeScheduleRepository"></param>
    /// <param name="placeRepository"></param>
    /// <param name="teamRepository"></param>
    /// <param name="timeProvider"></param>
    public PlaceScheduleUsecase(IDbConnectionProvider dbConnectionProvider, IPlaceScheduleRepository placeScheduleRepository,
                                IPlaceRepository placeRepository, ITeamRepository teamRepository,
                                TimeProvider timeProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _errorObjects = new List<ErrorObject>();
        _placeScheduleRepository = placeScheduleRepository;
        _placeRepository = placeRepository;
        _teamRepository = teamRepository;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 会場日程を登録する
    /// </summary>
    /// <param name="placeSchedules"></param>
    /// <returns></returns>
    public async Task<List<ErrorObject>> StorePlaceSchedulesAsync(List<PlaceSchedule> placeSchedules)
    {
        _errorObjects.Clear();

        // 必須チェック済みのリストを取得する
        var insertPlaceSchedulesByRequired = GetCheckedRequired(placeSchedules);

        // キー重複の確認
        var insertPlaceSchedulesByDuplicated = GetCheckedDuplicateKey(placeSchedules);

        // 既存会場コードの確認
        var insertPlaceSchedulesByPlaces = await GetCheckedPlaceCodes(placeSchedules);

        // 既存班コードの確認
        var insertPlaceSchedulesByTeams = await GetCheckedTeamCodes(placeSchedules);

        var commonInsertPlaceSchedules = insertPlaceSchedulesByRequired.Intersect(insertPlaceSchedulesByPlaces)
                                                                       .Intersect(insertPlaceSchedulesByTeams)
                                                                       .Intersect(insertPlaceSchedulesByDuplicated)
                                                                       .ToList();

        // 会場IDの取得
        var places = await _placeRepository.GetPlaceInfoAsync(commonInsertPlaceSchedules.Select(p => p.PlaceCode).ToList());

        // 班IDの取得
        var teams = await _teamRepository.GetTeamInfoAsync(commonInsertPlaceSchedules.Select(t => t.TeamCode).ToList());

        // 会場日程エンティティリストを生成
        var placeScheduleEntities = commonInsertPlaceSchedules.Select(placeSchedule => new PlaceScheduleEntity
        {
            PlaceId = places.Where(p => p.PlaceCode == placeSchedule.PlaceCode).Select(p => p.PlaceId).FirstOrDefault(),
            TeamId = teams.Where(t => t.TeamCode == placeSchedule.TeamCode).Select(t => t.TeamId).FirstOrDefault(),
            Status = (int)PlaceScheduleLockingStatus.検査中,
            ExamDate = DateOnly.FromDateTime(placeSchedule.ExamDate),
            StartTime = placeSchedule.ExamDate.ToString("HHmm")
        }).ToList();

        var createdAt = _timeProvider.GetUtcNow();
        string createdBy = "ExternalConnection";

        // 会場日時を登録する
        await _placeScheduleRepository.UpsertPlaceScheduleAsync(placeScheduleEntities, createdAt, createdBy);

        return _errorObjects;
    }

    /// <summary>
    /// 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="placeSchedules"></param>
    /// <returns></returns>
    private List<PlaceSchedule> GetCheckedRequired(List<PlaceSchedule> placeSchedules)
    {
        // WARNING検証
        // 未入力
        var requiredPlaceCodeData = placeSchedules.Where(x => string.IsNullOrWhiteSpace(x.PlaceCode));
        if (requiredPlaceCodeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredPlaceCodeData, "PlaceCode");
        }

        // 未入力
        var requiredTeamCodeData = placeSchedules.Where(x => string.IsNullOrWhiteSpace(x.TeamCode));
        if (requiredTeamCodeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredTeamCodeData, "TeamCode");
        }

        return placeSchedules.Except(requiredPlaceCodeData)
                             .Except(requiredTeamCodeData)
                             .ToList();
    }

    /// <summary>
    /// キー重複チェック処理済の会場日程リストを取得する。
    /// </summary>
    /// <param name="placeSchedules"></param>
    /// <returns></returns>
    private List<PlaceSchedule> GetCheckedDuplicateKey(List<PlaceSchedule> placeSchedules)
    {
        // WARNING検証
        // キー重複
        var duplicateKeys = placeSchedules.GroupBy(x => new { x.TeamCode, x.PlaceCode, ExamDate = DateOnly.FromDateTime(x.ExamDate) })
                                          .Where(x => x.Count() > 1)
                                          .Select(x => x.Key).ToHashSet();

        if (duplicateKeys.Any())
        {
            var duplicatedData = placeSchedules.Where(x => duplicateKeys.Contains(new { x.TeamCode, x.PlaceCode, ExamDate = DateOnly.FromDateTime(x.ExamDate) }));

            // 返却用エラーオブジェクトに追加
            AddDuplicateErrorObjects(duplicatedData);
            return placeSchedules.Except(duplicatedData).ToList();
        }
        else
        {
            return new List<PlaceSchedule>(placeSchedules);
        }
    }

    /// <summary>
    /// 会場コードチェック処理済の会場日程リストを取得する。
    /// </summary>
    /// <param name="placeSchedules"></param>
    /// <returns></returns>
    private async Task<List<PlaceSchedule>> GetCheckedPlaceCodes(List<PlaceSchedule> placeSchedules)
    {
        var results = new List<PlaceSchedule>();

        // WARNING検証
        // 会場コード
        var errorPlaceCodes = await PlaceCodesExists(placeSchedules);

        if (errorPlaceCodes.Count > 0)
        {
            // 返却用エラーオブジェクトに追加
            AddPlaceErrorObjects(placeSchedules, errorPlaceCodes);
            // 会場コードが存在する会場日程のみ抽出
            results = placeSchedules
                .Where(placeSchedule => !errorPlaceCodes.Contains(placeSchedule.PlaceCode))
                .ToList();
        }
        else
        {
            results = new List<PlaceSchedule>(placeSchedules);
        }
        return results;
    }

    /// <summary>
    /// 班コードチェック処理済の会場日程リストを取得する。
    /// </summary>
    /// <param name="placeSchedules"></param>
    /// <returns></returns>
    private async Task<List<PlaceSchedule>> GetCheckedTeamCodes(List<PlaceSchedule> placeSchedules)
    {
        var results = new List<PlaceSchedule>();

        // WARNING検証
        // 班コード
        var errorTeamCodes = await TeamCodesExists(placeSchedules);

        if (errorTeamCodes.Count > 0)
        {
            // 返却用エラーオブジェクトに追加
            AddTeamErrorObjects(placeSchedules, errorTeamCodes);
            // 班コードが存在する会場日程のみ抽出
            results = placeSchedules
                .Where(placeSchedule => !errorTeamCodes.Contains(placeSchedule.TeamCode))
                .ToList();
        }
        else
        {
            results = new List<PlaceSchedule>(placeSchedules);
        }
        return results;
    }

    /// <summary>
    /// 既存の会場コードを確認する
    /// </summary>
    /// <param name="placeSchedules"></param>
    /// <returns></returns>
    private async Task<List<string>> PlaceCodesExists(List<PlaceSchedule> placeSchedules)
    {
        var results = new List<string>();

        // 会場コードを取得する
        var placeCodes = placeSchedules
            .Select(placeSchedule => placeSchedule.PlaceCode)
            .Distinct()
            .ToList();

        // 会場コードを基に会場情報を取得する
        var existPlaceCodes = await _placeRepository.GetPlacesByCodesAsync(placeCodes);
        // 存在しない会場コードを取得する
        results = placeCodes.Except(existPlaceCodes).ToList();

        return results;
    }

    /// <summary>
    /// 既存の班コードを確認する
    /// </summary>
    /// <param name="placeSchedules"></param>
    /// <returns></returns>
    private async Task<List<string>> TeamCodesExists(List<PlaceSchedule> placeSchedules)
    {
        var results = new List<string>();

        // 班コードを取得する
        var teamCodes = placeSchedules
            .Select(placeSchedule => placeSchedule.TeamCode)
            .Distinct()
            .ToList();

        // 班コードを基に会場情報を取得する
        var existTeamCodes = await _teamRepository.GetTeamsByCodesAsync(teamCodes);
        // 存在しない班コードを取得する
        results = teamCodes.Except(existTeamCodes).ToList();

        return results;
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(必須項目エラー）
    /// </summary>
    /// <param name="requiredData"></param>
    /// <param name="itemName"></param>
    private void AddRequiredDataErrorObjects(IEnumerable<PlaceSchedule> requiredData, string itemName)
    {
        var errorObjects = requiredData
            .Select(r => new ErrorObject
            {
                Code = "10004",
                Message = $"必須項目が不足しています。{itemName}",
                InputNote = r.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する
    /// </summary>
    /// <param name="duplicatedData"></param>
    private void AddDuplicateErrorObjects(IEnumerable<PlaceSchedule> duplicatedData)
    {
        var errorObjects = duplicatedData
            .Select(d => new ErrorObject
            {
                Code = "10003",
                Message = $"キー項目が重複しています。TeamCode:{d.TeamCode}/PlaceCode:{d.PlaceCode}/ExamDate:{d.ExamDate}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(会場コード）
    /// </summary>
    /// <param name="placeSchedules">会場日程情報</param>
    /// <param name="errorPlaceCodes">会場取得に失敗した会場コードリスト</param>
    private void AddPlaceErrorObjects(IEnumerable<PlaceSchedule> placeSchedules, List<string> errorPlaceCodes)
    {
        // 取得できないエラーを返却用エラーオブジェクトに追加
        var errorObjects = placeSchedules
            .Where(placeSchedule => errorPlaceCodes.Contains(placeSchedule.PlaceCode))
            .Select(placeSchedule => new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたPlaceCodeがシステム上に存在しません。Code:{placeSchedule.PlaceCode}",
                InputNote = placeSchedule.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(班コード）
    /// </summary>
    /// <param name="placeSchedules">会場日程情報</param>
    /// <param name="errorTeamCodes">会場取得に失敗した班コードリスト</param>
    private void AddTeamErrorObjects(IEnumerable<PlaceSchedule> placeSchedules, List<string> errorTeamCodes)
    {
        // 取得できないエラーを返却用エラーオブジェクトに追加
        var errorObjects = placeSchedules
            .Where(placeSchedule => errorTeamCodes.Contains(placeSchedule.TeamCode))
            .Select(placeSchedule => new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたTeamCodeがシステム上に存在しません。Code:{placeSchedule.TeamCode}",
                InputNote = placeSchedule.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }
}
