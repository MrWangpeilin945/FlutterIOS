using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 検査機器リポジトリ
/// </summary>
public class EquipmentRepository : IEquipmentRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public EquipmentRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// 検査メニューを指定して検査機器リストを取得します。
    /// </summary>
    public async Task<IEnumerable<Equipment>> GetEquipmentAsync(int examMenuId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            equipment_id as EquipmentId
            , name as EquipmentName
            , app_launch_url as AppLaunchUrl
            , processing_script_url as ProcessingScriptUrl
        from
            resultcollector.equipments
        where
            exam_menu_id = @ExamMenuId
        order by
            EquipmentId;";

        var equipments = await connection.QueryAsync<Equipment>(sql, new { ExamMenuId = examMenuId });
        return equipments;
    }
}
