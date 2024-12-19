using Dapper;

using System.Data.Common;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 職員リポジトリ
/// </summary>
public class ResultRepository : IResultRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public ResultRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 検査結果を登録する
    /// </summary>
    public async Task RegisterResultsAsync(Guid consultId, ExamResultRegisteEntity[] results)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            var operationTime = DateTime.UtcNow;
            var saveItems = new List<object>();
            foreach (var detailResult in results)
            {
                var item = new
                {
                    ConsultId = consultId,
                    ExamItemDetailId = detailResult.ExamItemDetailId,
                    Value = detailResult.Value,
                    CreatedAt = operationTime, // TODO: システム時刻を取るサービスを作る 
                    CreatedBy = "作成者（仮）" // TODO: JWTから操作者の情報を取得する
                };
                saveItems.Add(item);
            }

            // 検査結果を登録
            const string mergeSql = @"
            merge 
            into resultcollector.exam_results as er 
                using (values (@ConsultId, @ExamItemDetailId, @Value, @CreatedAt, @CreatedBy)) as new_data( 
                    consult_id
                    , exam_item_detail_id
                    , value
                    , created_at
                    , created_by
                ) 
                    on er.consult_id = new_data.consult_id 
                    and er.exam_item_detail_id = new_data.exam_item_detail_id when matched then update 
            set
                value = new_data.value 
                , created_at = new_data.created_at
                , created_by = new_data.created_by when not matched then 
            insert ( 
                consult_id
                , exam_item_detail_id
                , value
                , created_at
                , created_by
            ) 
            values ( 
                new_data.consult_id
                , new_data.exam_item_detail_id
                , new_data.value
                , new_data.created_at
                , new_data.created_by
            );";
            await connection.ExecuteAsync(mergeSql, saveItems);

            // 検査結果履歴を登録
            const string insertSql = @"
            insert into resultcollector.exam_result_histories
            (
                consult_id
                , exam_item_detail_id
                , value
                , created_at
                , created_by
            )
            values
            (
                @ConsultId
                , @ExamItemDetailId
                , @Value
                , @CreatedAt
                , @CreatedBy
            );";
            await connection.ExecuteAsync(insertSql, saveItems);
            await transaction.CommitAsync();
        }
        catch(DbException)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}
