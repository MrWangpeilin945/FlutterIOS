using Dapper;

using System.Data.Common;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.Core.Exceptions;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 検査結果リポジトリ
/// </summary>
public class ResultRepository : IResultRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly IStaffIdentityProvider _staffIdentityProvider;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    /// <param name="staffIdentityProvider">職員情報プロバイダ</param>
    /// <param name="timeProvider">timeProvider</param>
    public ResultRepository(IDbConnectionProvider dbConnectionProvider, IStaffIdentityProvider staffIdentityProvider, TimeProvider timeProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _staffIdentityProvider = staffIdentityProvider;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 検査結果を登録する
    /// </summary>
    public async Task RegisterResultsAsync(Guid consultId, ExamResultRegisteEntity[] results)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        var operationTime = _timeProvider.GetUtcNow();
        var operationStaffCode = _staffIdentityProvider.StaffCode;
        if (operationStaffCode is null)
        {
            throw new WellshipAuthenticationException();
        }

        var param = results.Select(x => new
        {
            ConsultId = consultId,
            ExamItemDetailId = x.ExamItemDetailId,
            Value = x.Value,
            CreatedAt = operationTime,
            CreatedBy = operationStaffCode
        });

        // 検査結果を登録する
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
        await connection.ExecuteAsync(mergeSql, param);
    }

    /// <summary>
    /// 検査結果登録時のログを記録する
    /// </summary>
    public async Task WriteResultsLogAsync(Guid consultId, ExamResultRegisteEntity[] results)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        var operationTime = _timeProvider.GetUtcNow();
        var operationStaffCode = _staffIdentityProvider.StaffCode;
        if (operationStaffCode is null)
        {
            throw new WellshipAuthenticationException();
        }

        var param = results.Select(x => new
        {
            ConsultId = consultId,
            ExamItemDetailId = x.ExamItemDetailId,
            Value = x.Value,
            CreatedAt = operationTime,
            CreatedBy = operationStaffCode
        });

        // 検査結果履歴を登録する
        const string sql = @"
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

        await connection.ExecuteAsync(sql, param);
    }
}
