using System.Net;

namespace Ryobi.Wellship.Core.Exceptions;

/// <summary>
/// リソースが存在しない例外
/// </summary>
public class ResourceNotFoundException : WellshipException
{
    /// <summary>
    /// メッセージを付けて例外オブジェクトを生成します。
    /// </summary>
    public ResourceNotFoundException(string? message) : base(message) { }

    /// <summary>
    /// 引数なしで例外オブジェクトを生成します。
    /// </summary>
    public ResourceNotFoundException() { }

    /// <summary>
    /// 例外ハンドリングミドルウェアで返すHTTPステータスコード
    /// </summary>
    public override HttpStatusCode HttpStatusCode => HttpStatusCode.NotFound;

    /// <summary>
    /// エラーの内容を公開するURL
    /// </summary>
    public override string ErrorTypeUrl => @"https://tools.ietf.org/html/rfc7231#section-6.5.4";

    /// <summary>
    /// エラーのタイトル
    /// </summary>
    public override string ErrorTitle => "Resource NotFound";
}
