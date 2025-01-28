namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Logger;

/// <summary>
/// ロギングサービスのインターフェース
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// ログを書き込みます。
    /// </summary>
    /// <param name="message">メッセージ</param>
    void Log(string message);
}
