namespace Ryobi.Wellship.WebAPI.ResultCollector.Services;

/// <summary>
/// 連携処理結果をメール通知するためのサービスのインターフェース
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// 通知メールを送信します。
    /// 通知ルールや通知先の取得、送信履歴の書き込みまで一連の処理を担います。
    /// </summary>
    public Task SendNotificationEmailsAsync();
}
