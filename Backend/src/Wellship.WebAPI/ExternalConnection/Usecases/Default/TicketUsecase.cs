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
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="ticketRepository"></param>
    /// <param name="timeProvider"></param>
    public TicketUsecase(ITicketRepository ticketRepository, TimeProvider timeProvider)
    {
        _ticketRepository = ticketRepository;
        _errorObjects = new List<ErrorObject>();
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// EC2002_受付を更新する
    /// </summary>
    /// <param name="tickets">更新する受付のリスト</param>
    /// <returns>エラーリスト</returns>
    public async Task<List<ErrorObject>> StoreTicketsAsync(List<Ticket> tickets)
    {
        _errorObjects.Clear();

        // 必須チェック済みのリストを取得する
        var insertTicketsByRequired = GetCheckedRequired(tickets);

        // キー重複の確認
        var insertTicketsByDuplicated = GetCheckedDuplicateKey(tickets);

        // 登録する受付リストの受診IDを取得する
        var ticketConsultEntities = await _ticketRepository.GetTicketsAsync(tickets);
        // 受付可能な連携キー
        var connectionCodes = ticketConsultEntities.Select(x => x.ConnectionCode).ToArray();
        // 連携キーの取得に失敗した受付リスト
        var errorObjects = tickets.Where(x => !connectionCodes.Contains(x.ConnectionCode))
                                  .Select(x => new ErrorObject
                                    {
                                        Code = "10001",
                                        Message = $"指定されたConnectionCodeがシステム上に存在しません。Code:{x.ConnectionCode}",
                                        InputNote = x.InputNote
                                    }).ToList();
        _errorObjects.AddRange(errorObjects);
        // 連携キーの取得に成功した受付リスト
        var insertTicketsByConnectionCode = tickets.Where(x => connectionCodes.Contains(x.ConnectionCode))
                                            .Select(x => x);

        // 登録可能な受付情報
        var insertTickets = insertTicketsByRequired.Intersect(insertTicketsByDuplicated)
                                                   .Intersect(insertTicketsByConnectionCode);
        // 受付エンティティリストを生成
        var ticketsEntities = insertTickets.Select(x => new TicketEntity
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
        return _errorObjects;
    }

    /// <summary>
    /// 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns></returns>
    private List<Ticket> GetCheckedRequired(List<Ticket> tickets)
    {
        // WARNING検証
        // 未入力
        var requiredConnectionCodeData = tickets.Where(x => string.IsNullOrWhiteSpace(x.ConnectionCode ));
        if (requiredConnectionCodeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredConnectionCodeData, "ConnectionCode");
        }

        // 未入力
        var requiredTicketNumberData = tickets.Where(x => string.IsNullOrWhiteSpace(x.TicketNumber));
        if (requiredTicketNumberData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredTicketNumberData, "TicketNumber");
        }

        return tickets.Except(requiredConnectionCodeData)
                      .Except(requiredTicketNumberData)
                      .ToList();
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(必須項目エラー）
    /// </summary>
    /// <param name="requiredData"></param>
    /// <param name="itemName"></param>
    private void AddRequiredDataErrorObjects(IEnumerable<Ticket> requiredData, string itemName)
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
    /// キー重複の確認
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns></returns>
    private List<Ticket> GetCheckedDuplicateKey(List<Ticket> tickets)
    {
        // キー重複
        var duplicateSortNos = tickets.GroupBy(x => x.SortNo)
                                              .Where(x => x.Count() > 1)
                                              .SelectMany(x => x);
        if (duplicateSortNos.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddDuplicateSortNoErrorObjects(duplicateSortNos.ToList());
        }

        return tickets.Except(duplicateSortNos)
                      .ToList();
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(処理順重複エラー）
    /// </summary>
    /// <param name="duplicatedData"></param>
    private void AddDuplicateSortNoErrorObjects(IEnumerable<Ticket> duplicatedData)
    {
        var errorObjects = duplicatedData
            .Select(d => new ErrorObject
            {
                Code = "10003",
                Message = $"キー項目が重複しています。SortNo:{d.SortNo}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }
}