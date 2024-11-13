using Dapper;

using Ryobi.Wellship.WebAPI.DapperSample.Domain.Models;
using Ryobi.Wellship.WebAPI.DapperSample.Infrastructure.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

namespace Ryobi.Wellship.WebAPI.DapperSample.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// PgUserにオーナーになっているPgTablesを紐づけて返すサンプルのリポジトリです。
/// </summary>
public interface IPgUserRepository
{
    /// <summary>
    /// pg_userにpg_tablesを紐づけて返すサンプル
    /// </summary>
    /// <returns>ユーザー・テーブル情報</returns>
    public ValueTask<User[]> GetAllUsersAsync();
}

internal class PgUserRepository(IDbConnectionProvider dbConnectionProvider) : IPgUserRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;

    /// <summary>
    /// pg_userにpg_tablesを紐づけて返すサンプル
    /// </summary>
    /// <remarks>
    /// 複数のテーブルをjoin（特に1:n）して取得する場合はこのように書くことになります。
    /// 1:1であれば結合後整形した形にバインドする方法を取ったり、
    /// 主キーを指定しての1:nであればクエリを複数個投げて手で集計する等で回避可能です。
    /// </remarks>
    /// <returns></returns>
    public async ValueTask<User[]> GetAllUsersAsync()
    {
        const string query = @"select * from pg_user pu left join pg_tables pt on pt.tableowner = pu.usename order by pu.usename";

        // 接続情報に基づいて適したdbConnectionをプール・渡してくれるIDbConnectionProviderを用意しています。
        // (リクエスト中でコネクションを開いている場合はそれを返し、開いていない場合はコネクションプールから取ってきます)
        // ここでdisposeした場合は使用後即コネクションプールに戻しますし、disposeしなかった場合はレスポンスを返した後コネクションプールに戻します。
        // トランザクションを張っている場合はdisposeせず、トランザクションを張らない場合はその場でdisposeする方法を考えていますがまだうまくいっていません。
        var connection = await _dbConnectionProvider.GetOrOpenAsync();

        // QueryAsync
        // 第1～n-1型引数: クエリの結果から受け取る型（左から順）
        //                今回のクエリでは結合順からPgUserの列のあとにPgTableの列が並ぶのでPgUser, PgTableの順
        // 第n型引数     : QueryAsyncから返す型（第2引数のラムダで返す型）
        // 第1引数       : クエリ
        // 第2引数       : 第1～n-1型引数で指定した型の変数を引数として、第n型引数で指定した型を返すラムダ
        //                 クエリ結果の行ごとにこれが実行されるイメージです
        // splitOn       : 複数の型で受け取る場合の区切りとして使える列名（今回はPgUser→PgTableの順に列を受け取っており、PgTableの先頭列はschemanameのためschemanameを指定）
        //                 3つ以上の型で受け取る場合はカンマ区切りで指定したはず…
        var result = await connection.QueryAsync<PgUser, PgTable, (PgUser user, PgTable table)>(query, (user, table) => (user, table), splitOn: "schemaname");

        // userをrecord classにしているためuserでgroupbyしています。
        // 実際は主キーでGroupByするとよいです
        return result.GroupBy(x => x.user, x => x.table)
                     .Select(x => new User()
                     {
                         Name = x.Key.UseName,
                         SysId = x.Key.UseSysId,
                         UseCreateDb = x.Key.UseCreateDb,
                         UseSuper = x.Key.UseSuper,
                         UseRepl = x.Key.UseRepl,
                         UseByPassRls = x.Key.UseByPassRls,
                         Tables = x.Select(x => x).OfType<PgTable>().Select(x => new Table()
                         {
                             Name = x.TableName,
                             SchemaName = x.SchemaName,
                             HasIndex = x.HasIndexes,
                             HasRules = x.HasRules,
                             HasTriggers = x.HasTriggers,
                             RowSecurity = x.RowSecurity
                         }).ToArray()
                     }).ToArray();
        // ドメインモデルとか使う場合、結局マッピングを書きます
    }
}