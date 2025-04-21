namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 通知テンプレート
/// </summary>
public sealed class NotificationTemplate
{
    /// <summary>
    /// テンプレートID
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// テンプレート名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 件名
    /// </summary>
    public required string Subject { get; init; }

    /// <summary>
    /// 本文
    /// </summary>
    public required string Body { get; init; }

    /// <summary>
    /// 送信者アドレス
    /// </summary>
    public required string SenderAddress { get; init; }

    /// <summary>
    /// 通知グループID
    /// </summary>
    public required int NotificationGroupId { get; init; }

    /// <summary>
    /// 件名のプレースホルダを埋める
    /// </summary>
    /// <param name="placeholders">プレースホルダとその値の辞書</param>
    /// <returns>プレースホルダが埋められた件名</returns>
    public string FillSubjectPlaceholders(Dictionary<string, string> placeholders)
    {
        return FillPlaceholders(Subject, placeholders);
    }

    /// <summary>
    /// 本文のプレースホルダを埋める
    /// </summary>
    /// <param name="placeholders">プレースホルダとその値の辞書</param>
    /// <returns>プレースホルダが埋められた本文</returns>
    public string FillBodyPlaceholders(Dictionary<string, string> placeholders)
    {
        return FillPlaceholders(Body, placeholders);
    }

    /// <summary>
    /// プレースホルダを埋める共通メソッド
    /// </summary>
    /// <param name="template">テンプレート文字列</param>
    /// <param name="placeholders">プレースホルダとその値の辞書</param>
    /// <returns>プレースホルダが埋められたテンプレート文字列</returns>
    private static string FillPlaceholders(string template, Dictionary<string, string> placeholders)
    {
        string filledTemplate = template;
        foreach (var placeholder in placeholders)
        {
            filledTemplate = filledTemplate.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }
        return filledTemplate;
    }
}
