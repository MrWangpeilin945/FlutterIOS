
using System.Text.Json;

using Dapper;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// ホームメニューリポジトリ
/// /// </summary>
public class HomeMenuRepository : IHomeMenuRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// リポジトリを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    public HomeMenuRepository(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    /// <summary>
    /// ホームメニューグループ一覧を取得します。
    /// </summary>
    public async Task<IEnumerable<Domain.Models.HomeMenuGroup>> GetHomeMenuGroupsAsync()
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            p.home_menu_group_id as GroupId
            , p.name as GroupName
            , p.order_number as GroupOrderNumber
            , c.home_menu_id as MenuId
            , c.order_number as MenuOrderNumber
            , c.name as MenuName
            , c.path 
        from
            resultcollector.home_menu_groups as p 
            left join resultcollector.home_menus as c 
                on p.home_menu_group_id = c.home_menu_group_id 
        order by
            p.order_number
            , c.order_number;";

        var results = await connection.QueryAsync<HomeMenuGroupEntity>(sql);
        return results.OrderBy(c => c.GroupOrderNumber)
                      .ThenBy(c => c.MenuOrderNumber)
                      .Select(c => new Domain.Models.HomeMenuGroup
                      {
                          GroupId = c.GroupId,
                          GroupName = c.GroupName,
                          HomeMenus = new List<Domain.Models.HomeMenu>
                          {
                              new Domain.Models.HomeMenu
                              {
                                  MenuId = c.MenuId,
                                  MenuName = c.MenuName,
                                  Path = c.Path
                              }
                          }
                      }).ToList();
    }
}
