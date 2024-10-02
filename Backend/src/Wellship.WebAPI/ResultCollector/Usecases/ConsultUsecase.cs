using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診ユースケース
/// </summary>
public class ConsultUsecase : IConsultUsecase
{
    private readonly IConsultRepository _consultRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    public ConsultUsecase(IConsultRepository consultRepository)
    {
        _consultRepository = consultRepository;
    }

    /// <summary>
    /// 予約Noの受診が存在するか確認する
    /// </summary>
    /// <param name="reservationNoRequest">予約Noリクエスト</param>
    /// <returns>受診が存在するか</returns>
    public void VerifyReservationNo(ReservationNoRequest reservationNoRequest)
    {
        var reservationNo = reservationNoRequest.ReservationNo;
        var consultExists = _consultRepository.ConsultExists(reservationNo);

        if (!consultExists)
        {
            throw new ReservationNoNotFoundException("予約Noが存在しません。");
        }
    }

    /// <summary>
    /// 未受診の健診メニューを取得する
    /// </summary>
    public void GetUnexaminedMenus()
    {

    }

    /// <summary>
    /// 簡易な受診者情報を取得する
    /// </summary>
    public void GetSimpleExaminee()
    {

    }

    /// <summary>
    /// 詳細な受診者情報を取得する
    /// </summary>
    public void GetDetailedExaminee()
    {

    }

    /// <summary>
    /// 検査結果の連携状態を変更する
    /// </summary>
    public void ChangeIntegrationStatus()
    {
        
    }

}
