using Dapper;
using System.Data.Common;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.ExternalConnection.Enums;
using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;

/// <summary>
/// 受診を更新するRepository層
/// </summary>
public class ConsultRepository : IConsultRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public ConsultRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 受診を更新する
    /// </summary>
    /// <param name="consults">更新する受診リスト</param>
    /// <param name="createdAt">作成日時</param>
    /// <param name="createdBy">作成者</param>
    public async Task UpsertConsultsAsync(List<string> consults, DateTime createdAt, string createdBy)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
        }
        catch(DbException)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// 存在する検査メニュー特記コード情報（検査特記コード、検査特記名）を取得する
    /// </summary>
    /// <param name="codes">検査メニュー特記コードのリスト</param>
    public async Task<List<ExamMenuNoteCodeEntity>> GetExamMenuNodeCodesAsync(List<string> codes)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        var sql = @"
                select
                    code as Code
                    , name as Name
                from
                    resultcollector.exam_menu_note_codes
                where
                    code = any(@Codes);";

        var result = await connection.QueryAsync<ExamMenuNoteCodeEntity>(sql, new { Codes = codes });

        return result.ToList();
    }
}
