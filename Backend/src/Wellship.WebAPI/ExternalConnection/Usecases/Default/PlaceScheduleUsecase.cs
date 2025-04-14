using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.Core.Enums;
using PlaceSchedule = Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard.PlaceSchedule;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2012_会場日程を登録する
/// </summary>
public class PlaceScheduleUsecase : IPlaceScheduleUsecase
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
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
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        // 会場IDの取得
        var places = await _placeRepository.GetPlaceInfoAsync(placeSchedules.Select(x => x.PlaceCode).Distinct().ToList());

        // 班IDの取得
        var teams = await _teamRepository.GetTeamInfoAsync(placeSchedules.Select(x => x.TeamCode).Distinct().ToList());

        // WARNING検証
        var warningPlaceSchedules = new List<PlaceSchedule>();

        // 必須項目の空値のチェック
        var spaceCheckProperties = new[]
        {
            "TeamCode",     // 班コード,
            "PlaceCode"     // 会場コード
        };
        // チェックするプロパティ一覧をメソッドに渡して空値チェックする
        foreach (var warning in ValidationChecker.SpaceCheckProperties(placeSchedules, spaceCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをPlaceScheduleにキャストしてワーニングリストに追加する
            if (warning is PlaceSchedule placeSchedule)
            {
                warningPlaceSchedules.Add(placeSchedule);
            }
        }

        // ユニークキーが重複するレコードのチェック
        // PlaceCode+TeamCode+ExamDateで重複する
        var duplicatePlaceScheduleKeys = placeSchedules
                                                .GroupBy(x => new { x.PlaceCode, x.TeamCode, ExamDate = DateOnly.FromDateTime(x.ExamDate) })
                                                .Where(x => x.Count() > 1)
                                                .Select(x => x.Key).ToHashSet();
        if (duplicatePlaceScheduleKeys.Any())
        {
            // PKが重複したレコードを取得する
            var duplicatePlaceSchedules = placeSchedules.Where(x => duplicatePlaceScheduleKeys.Any(duplicateKey =>
                                                            duplicateKey.PlaceCode == x.PlaceCode &&
                                                            duplicateKey.TeamCode == x.TeamCode &&
                                                            duplicateKey.ExamDate == DateOnly.FromDateTime(x.ExamDate)
                                                        ));
            foreach (var warning in duplicatePlaceSchedules)
            {
                warningPlaceSchedules.Add(warning);
                errorObjects.Add(new ErrorObject
                {
                    Code = "10003",
                    Message = $"キー項目が重複しています。TeamCode:{warning.TeamCode}/PlaceCode:{warning.PlaceCode}/ExamDate:{warning.ExamDate.ToString("yyyy/MM/dd")}",
                    InputNote = warning.InputNote
                });
            }
        }

        // 会場コードの確認
        foreach (var warning in placeSchedules.Where(x => !places.Select(p => p.PlaceCode).Contains(x.PlaceCode)))
        {
            warningPlaceSchedules.Add(warning);
            errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたPlaceCodeがシステム上に存在しません。Code:{warning.PlaceCode}",
                InputNote = warning.InputNote
            });
        }

        // 班コードの確認
        foreach (var warning in placeSchedules.Where(x => !teams.Select(t => t.TeamCode).Contains(x.TeamCode)))
        {
            warningPlaceSchedules.Add(warning);
            errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたTeamCodeがシステム上に存在しません。Code:{warning.TeamCode}",
                InputNote = warning.InputNote
            });
        }

        // 会場日程エンティティリストを生成
        var placeScheduleEntities = placeSchedules.Except(warningPlaceSchedules)
                                                  .Select(placeSchedule => new PlaceScheduleEntity
                                                  {
                                                      PlaceId = places.Where(p => p.PlaceCode == placeSchedule.PlaceCode).Select(p => p.PlaceId).FirstOrDefault(),
                                                      TeamId = teams.Where(t => t.TeamCode == placeSchedule.TeamCode).Select(t => t.TeamId).FirstOrDefault(),
                                                      Status = (int)PlaceScheduleLockingStatus.検査中,
                                                      ExamDate = DateOnly.FromDateTime(placeSchedule.ExamDate),
                                                      StartTime = placeSchedule.ExamDate.ToString("HHmm")
                                                  }).ToList();

        // 会場日時を登録する
        await _placeScheduleRepository.UpsertPlaceScheduleAsync(placeScheduleEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return errorObjects;
    }
}
