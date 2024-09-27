namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 受診リポジトリ
/// </summary>
public interface IConsultRepository
{
    /// <summary>
    /// 受診が存在するか
    /// </summary>
    /// <param name="reservationNo">予約No</param>
    public bool ConsultExists(string reservationNo);
}
