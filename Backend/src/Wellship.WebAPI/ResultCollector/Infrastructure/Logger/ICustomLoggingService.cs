namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Logger;

/// <summary>
/// カスタムロギングサービスのインターフェース
/// アプリケーションから明示的に記録したいときに使います。
/// データベースに書き込みます。
/// ログレベルがERRORの場合はNLog経由でも出力します。
/// </summary>
public interface ICustomLoggingService
{
    /// <summary>
    /// INFOログを書き込みます。
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="details">付加情報</param>
    Task LogInfoAsync(string message, Dictionary<string, object>? details = null);

    /// <summary>
    /// WARNログを書き込みます。
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="details">付加情報</param>
    Task LogWarnAsync(string message, Dictionary<string, object>? details = null);

    /// <summary>
    /// ERRORログを書き込みます。
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="details">付加情報</param>
    Task LogErrorAsync(string message, Dictionary<string, object>? details = null);
}
