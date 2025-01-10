using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// ホームメニューの設定リスト
/// </summary>
public class HomeMenuSettings
{
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
            new HomeMenuSetting(){
                Path = ScreenPathConsts.検査メニュー選択画面,
                PlaceScheduleSelected = true,
                PlaceScheduleUnlocked = true,
                TargetRoles = [Role.User, Role.Admin]
            },
            new HomeMenuSetting(){
                Path = ScreenPathConsts.進捗画面,
                PlaceScheduleSelected = true,
                PlaceScheduleUnlocked = false,
                TargetRoles = [Role.User, Role.Admin]
            },
            new HomeMenuSetting(){
                Path = ScreenPathConsts.会場ロック画面,
                PlaceScheduleSelected = true,
                PlaceScheduleUnlocked = true,
                TargetRoles = [Role.Admin]
            },
            new HomeMenuSetting(){
                Path = ScreenPathConsts.検査結果出力画面,
                PlaceScheduleSelected = false,
                PlaceScheduleUnlocked = false,
                TargetRoles = [Role.Admin]
            },
            new HomeMenuSetting(){
                Path = ScreenPathConsts.検査結果出力履歴画面,
                PlaceScheduleSelected = false,
                PlaceScheduleUnlocked = false,
                TargetRoles = [Role.Admin]
            },
         ];
    }

    /// <summary>
    /// ホームメニューの利用可能条件を取得する。
    /// </summary>
    public string[] GetAvailableConditions(string path)
    {
        var setting = Settings.SingleOrDefault(x => x.Path == path);
        if (setting is null)
        {
            throw new ArgumentException($"設定された画面パスが無効です。{path}", nameof(path));
        }

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
    /// ロールがその機能を使用できるか判定する。
    /// </summary>
    public bool CanRoleUseFeature(string path, Role role)
    {
        var setting = Settings.SingleOrDefault(x => x.Path == path);
        if (setting is null)
        {
            throw new ArgumentException($"設定された画面パスが無効です。{path}", nameof(path));
        }

        return setting.TargetRoles.Contains(role);
    }
}
