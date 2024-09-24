using System.Net;

namespace Ryobi.Wellship.Core.Exceptions;

/// <summary>
/// WELLSHIP例外の基底クラス
/// </summary>
public abstract class WellshipException : Exception
{
    /// <summary>
    /// メッセージを付けて例外オブジェクトを生成します。
    /// </summary>
    public WellshipException(string? message) : base(message) { }

    /// <summary>
    /// 引数なしで例外オブジェクトを生成します。
    /// </summary>
    public WellshipException() { }

    /// <summary>
    /// 例外ハンドリングミドルウェアで返すHTTPステータスコード
    /// </summary>
    public virtual HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
    /// <summary>
    /// エラーの内容を公開するURL
    /// </summary>
    public virtual string ErrorTypeUrl => @"https://tools.ietf.org/html/rfc7231#section-6.5.1";
    /// <summary>
    /// エラーのタイトル
    /// </summary>
    public virtual string ErrorTitle => "Bad Request";
    /// <summary>
    /// レスポンス用エラー
    /// </summary>
    public virtual Dictionary<string, object>? Errors { get; set; }
}
