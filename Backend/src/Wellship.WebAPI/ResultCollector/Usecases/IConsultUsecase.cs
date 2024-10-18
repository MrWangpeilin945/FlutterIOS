using Ryobi.Wellship.APIModels.Requests;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診ユースケースのインターフェース
/// </summary>
public interface IConsultUsecase
{
    /// <summary>
    /// 受診番号の受診が存在するか確認する
    /// </summary>
    /// <param name="consultNumberRequest">受診番号リクエスト</param>
    /// <returns>受診が存在するか</returns>
    public void VerifyConsultNumber(ConsultNumberRequest consultNumberRequest);

    /// <summary>
    /// 未受診の健診メニューを取得する
    /// </summary>
    public void GetUnexaminedMenus();

    /// <summary>
    /// 簡易な受診者情報を取得する
    /// </summary>
    public void GetSimpleExaminee();

    /// <summary>
    /// 詳細な受診者情報を取得する
    /// </summary>
    public void GetDetailedExaminee();

    /// <summary>
    /// 検査結果の連携状態を変更する
    /// </summary>
    public void ChangeIntegrationStatus();
}
