using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
/// <summary>
/// EC2002_受付を更新する
/// </summary>
public class ConsultUsecase : IConsultUsecase
{
    private List<ErrorObject> _errorObjects;
    private readonly IConsultRepository _consultRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IPlaceRepository _placeRepository;
    private readonly IPlaceScheduleRepository _placeScheduleRepository;
    private readonly IExamineeRepository _examineeRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="teamRepository">班リポジトリ</param>
    /// <param name="placeRepository">会場リポジトリ</param>
    /// <param name="placeScheduleRepository">会場日程リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    public ConsultUsecase(IConsultRepository consultRepository, ITeamRepository teamRepository, IPlaceRepository placeRepository,
                          IPlaceScheduleRepository placeScheduleRepository, IExamineeRepository examineeRepository)
    {
        _consultRepository = consultRepository;
        _teamRepository = teamRepository;
        _placeRepository = placeRepository;
        _placeScheduleRepository = placeScheduleRepository;
        _examineeRepository = examineeRepository;
        _errorObjects = new List<ErrorObject>();
    }

    /// <summary>
    /// EC2004_受診を更新する
    /// </summary>
    /// <param name="consults">更新する受診のリスト</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreConsultAsync(List<Consult> consults)
    {
        // 班コードに紐づく班IDを取得する
        var teams = await _teamRepository.GetTeamInfoAsync(consults.Select(x => x.TeamCode).ToList());
        // 会場コードに紐づく会場IDを取得する
        var places = await _placeRepository.GetPlaceInfoAsync(consults.Select(x => x.PlaceCode).ToList());
        // 会場日程を取得する
        // 受診者コードに紐づく受診者IDを取得する
        var examinees = await _examineeRepository.GetExamineeInfoAsync(consults.Select(x => x.ExamineeCd).ToList());
        // 検査メニュー特記コードに紐づく情報を取得する
        var examMenuNodeCodes = await _consultRepository.GetExamMenuNodeCodesAsync(
                                            consults.SelectMany(x => x.ConsultNotes.Select(cn => cn.Code)).ToList());
        /*
        // 登録する受付リストの受診IDを取得する
        var ticketConsultEntities = await _ticketRepository.GetTicketsAsync(tickets);
        // 受付可能な連携キー
        var connectionCodes = ticketConsultEntities.Select(x => x.ConnectionCode).ToArray();
        // 連携キーの取得に失敗した受付リスト
        _errorObjects = tickets.Where(x => !connectionCodes.Contains(x.ConnectionCode))
                                .Select(x => new ErrorObject
                                {
                                    Code = "10001",
                                    Message = $"指定されたConnectionCodeがシステム上に存在しません。Code:[{x.ConnectionCode}]",
                                    InputNote = x.InputNote
                                }).ToList();
        // 連携キーの取得に成功した受付リスト
        var validTickets = tickets.Where(x => connectionCodes.Contains(x.ConnectionCode))
                                    .Select(x => new TicketEntity
                                    {
                                        ConsultId = ticketConsultEntities.Where(t => t.ConnectionCode == x.ConnectionCode)
                                                                         .Select(t => t.ConsultId).FirstOrDefault(),
                                        TicketNumber = x.TicketNumber,
                                        ConnectionCode = x.ConnectionCode,
                                        ActionType = x.ActionType,
                                        OrderNumber = x.SortNo          
                                    }).ToList();
        // 受付を更新する
        await _ticketRepository.UpsertTicketsAsync(validTickets, DateTime.Now, "ExternalConnection"); 
        */                       
        return _errorObjects;
    }
}