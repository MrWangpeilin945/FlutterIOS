using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.RepositoryImpls;

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
    /// <param name="reservationNo">予約No</param>
    public bool ConsultExists(string reservationNo)
    {
        // TODO: データベースと接続したら実装する

        // var sql = "select count(1) from consult where reservation_id = @ReservationNo";
        // _connector.Execute(sql, new { ReservationNo = reservationNo });
        // var result = _connector.Query<int>(sql).SingleOrDefault();
        // return result == 1;
        return true;
    }
}
