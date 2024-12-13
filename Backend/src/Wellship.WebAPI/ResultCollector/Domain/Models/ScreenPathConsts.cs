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

/// <summary>
/// ホームメニューの設定
/// </summary>
public class HomeMenuSetting
{

    /// <summary>
    /// 画面パス
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// 利用可能条件：会場日程が選択済み
    /// </summary>
    public required bool PlaceScheduleSelected { get; init; }

    /// <summary>
    /// 利用可能条件：会場ロックが解除されている
    /// </summary>
    public required bool PlaceScheduleUnlocked { get; init; }
}

/// <summary>
/// ホームメニューの設定リスト
/// </summary>
public class HomeMenuSettings
{
    /// <summary>
    /// ホームメニューの利用可能条件を取得する。
    /// </summary>
    public string[] GetAvailableConditions(string path)
    {
        var setting = Settings.Single(x => x.Path == path);
        var results = new List<string>();

        if (setting.PlaceScheduleUnlocked)
        {
            results.Add(nameof(setting.PlaceScheduleUnlocked));
        }
        if (setting.PlaceScheduleSelected)
        {
            results.Add(nameof(setting.PlaceScheduleSelected));
        }

        return results.ToArray();
    }

    /// <summary>
    /// 設定
    /// </summary>
    private static List<HomeMenuSetting> Settings
    {
        // PlaceScheduleSelected
        // - true  : 会場日程の選択が必要
        // - false : 会場日程の選択は不要
        // PlaceScheduleUnlocked
        // - true  : 会場ロックが解除されている
        // - false : 会場ロックの状態を問わない

        get => [
            new HomeMenuSetting(){Path = ScreenPathConsts.検査メニュー選択画面, PlaceScheduleSelected = true, PlaceScheduleUnlocked = false},
            new HomeMenuSetting(){Path = ScreenPathConsts.進捗画面, PlaceScheduleSelected = true, PlaceScheduleUnlocked = false},
            new HomeMenuSetting(){Path = ScreenPathConsts.会場ロック画面, PlaceScheduleSelected = true, PlaceScheduleUnlocked = false},
            new HomeMenuSetting(){Path = ScreenPathConsts.検査結果出力画面, PlaceScheduleSelected = false, PlaceScheduleUnlocked = false},
            new HomeMenuSetting(){Path = ScreenPathConsts.検査結果出力履歴画面, PlaceScheduleSelected = false, PlaceScheduleUnlocked = false},
         ];
    }
}
