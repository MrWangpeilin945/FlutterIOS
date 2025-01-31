using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2002_受付を更新する
/// </summary>
public class TicketUsecase : ITicketUsecase
{
    private List<ErrorObject> _errorObjects;
    private readonly ITicketRepository _ticketRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="ticketRepository"></param>
    public TicketUsecase(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
        _errorObjects = new List<ErrorObject>();
    }

    /// <summary>
    /// EC2002_受付を更新する
    /// </summary>
    /// <param name="tickets">更新する受付のリスト</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreTicketsAsync(List<Ticket> tickets)
    {
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
        return _errorObjects;
    }
}