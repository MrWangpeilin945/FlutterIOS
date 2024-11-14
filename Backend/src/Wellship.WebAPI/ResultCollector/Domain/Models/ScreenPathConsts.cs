namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// フロントエンドの画面名とパスを管理するクラス
/// </summary>
public class ScreenPathConsts
{
    /// <summary>
    /// SC0001_ログイン画面
    /// </summary>
    public static readonly string ログイン画面 = "login";

    /// <summary>
    /// SC0002_班選択画面
    /// </summary>
    public static readonly string 班選択画面 = "team-select";

    /// <summary>
    /// SC0003_会場選択画面
    /// </summary>
    public static readonly string 会場選択画面 = "place-select";

    /// <summary>
    /// SC0004_ホーム画面
    /// </summary>
    public static readonly string ホーム画面 = "home";

    /// <summary>
    /// SC0005_検査メニュー選択画面
    /// </summary>
    public static readonly string 検査メニュー選択画面 = "exammenu-select";

    /// <summary>
    /// SC0006_受診番号入力画面
    /// </summary>
    public static readonly string 受診番号入力画面 = "consultnumber-input";

    /// <summary>
    /// SC0007_検査内容確認画面
    /// </summary>
    public static readonly string 検査内容確認画面 = "examorder-confirm";

    /// <summary>
    /// SC0008_検査結果入力画面
    /// </summary>
    public static readonly string 検査結果入力画面 = "consult-input";

    /// <summary>
    /// SC0010_進捗画面
    /// </summary>
    public static readonly string 進捗画面 = "progress";

    /// <summary>
    /// SC0013_会場ロック画面
    /// </summary>
    public static readonly string 会場ロック画面 = "placeschedule-lock";

    /// <summary>
    /// SC0019_検査結果出力画面
    /// </summary>
    public static readonly string 検査結果出力画面 = "examresult-export";

    /// <summary>
    /// SC0020_検査結果出力履歴画面
    /// </summary>
    public static readonly string 検査結果出力履歴画面 = "examresult-export-history";
}