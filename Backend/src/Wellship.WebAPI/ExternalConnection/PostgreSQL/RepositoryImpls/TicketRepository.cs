using Dapper;
using System.Data.Common;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;

/// <summary>
/// 受付を更新するRepository層
/// </summary>
public class TicketRepository : ITicketRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public TicketRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 受付を更新する
    /// </summary>
    /// <param name="tickets">更新する受付リスト</param>
    /// <param name="createdAt">作成日時</param>
    /// <param name="createdBy">作成者</param>
    public async Task UpsertTicketsAsync(List<TicketEntity> tickets, DateTime createdAt, string createdBy)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            // 処理する受付リスト
            var actionTickets = tickets.GroupBy(x => x.ConnectionCode, 
                                               (y, z) => z.OrderByDescending(a => a.OrderNumber).First())
                                       .ToArray();
            // 受付を更新する
            var upsertItems = actionTickets.Where(x => x.ActionType == Wellship.ExternalConnection.Enums.ActionType.登録)
                                           .Select(x => new {
                                                ConsultId = x.ConsultId,
                                                TicketNumber = x.TicketNumber,
                                                CreatedAt = createdAt,
                                                CreatedBy = createdBy
                                            }).ToArray();
            const string mergeSql = @"
            merge
            into resultcollector.tickets as tc
                using (values (@ConsultId, @TicketNumber, @CreatedAt, @CreatedBy)) as new_data(
                    consult_id
                    , ticket_number
                    , created_at
                    , created_by
                )
                    on tc.consult_id = new_data.consult_id
                    when matched then update
            set
                ticket_number = new_data.ticket_number
                , created_at = new_data.created_at
                , created_by = new_data.created_by when not matched then
            insert (
                consult_id
                , ticket_number
                , created_at
                , created_by
            )
            values (
                new_data.consult_id
                , new_data.ticket_number
                , new_data.created_at
                , new_data.created_by
            );";
            await connection.ExecuteAsync(mergeSql, upsertItems);
            // 受付を削除する
            var deleteTickets = actionTickets.Where(x => x.ActionType == Wellship.ExternalConnection.Enums.ActionType.削除)
                                             .Select(x => x.ConsultId)
                                             .ToArray();
            const string deleteSql = @"
            delete
            from
                resultcollector.tickets
            where
                consult_id = any (@ConsultIds);";
            await connection.ExecuteAsync(deleteSql, new { ConsultIds = deleteTickets});
            // 受付履歴を登録する
            var historyItems = tickets.Select(x => new {
                                    ConsultId = x.ConsultId,
                                    TicketNumber = x.TicketNumber,
                                    ActionType = (int)x.ActionType,
                                    OrderNumber = x.OrderNumber,
                                    CreatedAt = createdAt,
                                    CreatedBy = createdBy
                                }).ToArray();
            const string historiesSql = @"
            insert into resultcollector.tickets_histories
            (
                consult_id
                , ticket_number
                , action_type
                , order_number
                , created_at
                , created_by
            )
            values
            (
                @ConsultId
                , @TicketNumber
                , @ActionType
                , @OrderNumber
                , @CreatedAt
                , @CreatedBy
            );";
            await connection.ExecuteAsync(historiesSql, historyItems);
            await transaction.CommitAsync();
        }
        catch(DbException)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    /// <summary>
    /// 更新対象の受付リストを取得する
    /// </summary>
    /// <param name="tickets">更新する受付のリスト</param>
    public async Task<IEnumerable<TicketConsultEntity>> GetTicketsAsync(List<Ticket> tickets)
    {
        var ticketNumbers = tickets.Select(x => x.TicketNumber).ToArray();
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            consult_id as ConsultId
            , external_connection_code as ConnectionCode
        from
            resultcollector.consult
        where
            external_connection_code = any (@TicketNumbers);";
        return await connection.QueryAsync<TicketConsultEntity>(sql, new { TicketNumbers = ticketNumbers });
    }
}
