using Ryobi.Wellship.APIModels.Requests;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診ユースケースのインターフェース
/// </summary>
public interface IConsultUsecase
{
    /// <summary>
    /// 予約Noの受診が存在するか確認する
    /// </summary>
    /// <param name="reservationNoRequest">予約Noリクエスト</param>
    /// <returns>受診が存在するか</returns>
    public void VerifyReservationNo(ReservationNoRequest reservationNoRequest);
}
