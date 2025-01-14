using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
/// <summary>
/// 受付を更新するRepository層
/// </summary>
public interface ITicketRepository
{
    /// <summary>
    /// 受付を更新する
    /// </summary>
    /// <param name="tickets">更新する受付リスト</param>
    /// <param name="createdAt">作成日時</param>
    /// <param name="createdBy">作成者</param>
    public Task UpsertTicketsAsync(List<TicketEntity> tickets, DateTime createdAt, string createdBy);

    /// <summary>
    /// 更新対象の受付リストを取得する
    /// </summary>
    /// <param name="tickets">更新する受付のリスト</param>
    public Task<IEnumerable<TicketConsultEntity>> GetTicketsAsync(List<Ticket> tickets);
}
