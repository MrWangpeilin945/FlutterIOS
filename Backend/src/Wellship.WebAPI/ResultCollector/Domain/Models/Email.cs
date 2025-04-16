namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// Eメール
/// </summary>
public sealed class Email
{
    /// <summary>
    /// 件名
    /// </summary>
    public required string Subject { get; init; }

    /// <summary>
    /// 本文
    /// </summary>
    public required string Body { get; init; }

    /// <summary>
    /// 通知先メールアドレスリスト
    /// </summary>
    public required List<string> Recipients { get; init; }

    /// <summary>
    /// 差出人メールアドレス
    /// </summary>
    public required string Sender { get; init; }
}
