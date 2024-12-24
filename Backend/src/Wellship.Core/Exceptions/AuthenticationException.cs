using System.Net;

namespace Ryobi.Wellship.Core.Exceptions;
/// <summary>
/// 認証情報に関する検証に失敗した場合の例外です
/// </summary>
public class WellshipAuthenticationException : WellshipException
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public WellshipAuthenticationException()
    {
    }
    /// <summary>
    /// エラーの内容を公開するURL
    /// </summary>
    public override string ErrorTypeUrl => @"https://tools.ietf.org/html/rfc7235#section-3.1";
    /// <summary>
    /// エラーのタイトル
    /// </summary>
    public override string ErrorTitle => "Authentication Failed";
    /// <summary>
    /// 例外メッセージ
    /// </summary>
    public override string Message => $"認証情報が検証できませんでした。";
    /// <summary>
    /// 対応するHttpStatusCode
    /// </summary>
    public override HttpStatusCode HttpStatusCode => HttpStatusCode.Unauthorized;
}
