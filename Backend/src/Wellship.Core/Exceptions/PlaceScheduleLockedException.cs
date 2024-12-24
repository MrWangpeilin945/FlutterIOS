using System.Net;

namespace Ryobi.Wellship.Core.Exceptions;

/// <summary>
/// 会場日程がロック状態の例外
/// </summary>
public class PlaceScheduleLockedException : WellshipException
{
    /// <summary>
    /// メッセージを付けて例外オブジェクトを生成します。
    /// </summary>
    public PlaceScheduleLockedException(string? message) : base(message) { }

    /// <summary>
    /// 引数なしで例外オブジェクトを生成します。
    /// </summary>
    public PlaceScheduleLockedException() { }

    /// <summary>
    /// 例外ハンドリングミドルウェアで返すHTTPステータスコード
    /// </summary>
    public override HttpStatusCode HttpStatusCode => HttpStatusCode.Forbidden;
    /// <summary>
    /// エラーの内容を公開するURL
    /// </summary>
    public override string ErrorTypeUrl => @"https://tools.ietf.org/html/rfc7231#section-6.5.4";
    /// <summary>
    /// エラーのタイトル
    /// </summary>
    public override string ErrorTitle => "PlaceSchedule Locked";
}
