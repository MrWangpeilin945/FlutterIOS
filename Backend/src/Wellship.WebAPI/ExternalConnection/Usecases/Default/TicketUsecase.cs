using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2002_受付を更新する
/// </summary>
public class TicketUsecase : ITicketUsecase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="ticketRepository"></param>
    /// <param name="timeProvider"></param>
    public TicketUsecase(ITicketRepository ticketRepository, TimeProvider timeProvider)
    {
        _ticketRepository = ticketRepository;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// EC2002_受付を更新する
    /// </summary>
    /// <param name="tickets">更新する受付のリスト</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreTicketsAsync(List<Ticket> tickets)
    {
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        // 登録する受付リストの受診IDを取得する
        var ticketConsultEntities = await _ticketRepository.GetTicketsAsync(tickets);

        // WARNING検証
        var warningTickets = new List<Ticket>();

        // 必須項目の空値のチェック
        var spaceCheckProperties = new[]
        {
            "ConnectionCode",   // 連携キー,
            "TicketNumber"      // 受付番号
        };
        // チェックするプロパティ一覧をメソッドに渡して空値チェックする
        foreach (var warning in ValidationChecker.SpaceCheckProperties(tickets, spaceCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをTicketにキャストしてワーニングリストに追加する
            if (warning is Ticket ticket)
            {
                warningTickets.Add(ticket);
            }
        }

        // キー重複チェック
        var duplicateCheckProperties = new[]
        {
            "SortNo"            // 処理順
        };
        // チェックするプロパティ一覧をメソッドに渡して重複チェックする
        foreach (var warning in ValidationChecker.DuplicateCheckProperties(tickets, duplicateCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをTicketにキャストしてワーニングリストに追加する
            if (warning is Ticket ticket)
            {
                warningTickets.Add(ticket);
            }
        }

        // 会場IDが取得できない
        foreach (var warning in tickets.Where(x => !ticketConsultEntities.Select(t => t.ConnectionCode).Contains(x.ConnectionCode)))
        {
            warningTickets.Add(warning);
            errorObjects.Add(new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたConnectionCodeがシステム上に存在しません。Code:{warning.ConnectionCode}",
                InputNote = warning.InputNote
            });
        }

        // 受付エンティティリストを生成
        var ticketsEntities = tickets.Except(warningTickets)
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
        await _ticketRepository.UpsertTicketsAsync(ticketsEntities, _timeProvider.GetUtcNow(), "ExternalConnection");
        
        return errorObjects;
    }
}