using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

/// <summary>
/// 受診リポジトリ
/// </summary>
public class ConsultRepository : IConsultRepository
{
    private readonly PostgresConnector _connector;

    /// <summary>
    /// 受診リポジトリを生成します。
    /// </summary>
    /// <param name="connector">PostgreSQL用コネクター</param>
    public ConsultRepository(PostgresConnector connector)
    {
        _connector = connector;
    }

    /// <summary>
    /// 受診が存在するか
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    public bool ConsultExists(string consultNumber)
    {
        // TODO: データベースと接続したら実装する

        // var sql = "select count(1) from consult where reservation_id = @ConsultNumber";
        // _connector.Execute(sql, new { ConsultNumber = consultNumber });
        // var result = _connector.Query<int>(sql).SingleOrDefault();
        // return result == 1;
        return true;
    }
}
