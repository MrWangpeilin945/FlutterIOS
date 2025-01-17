namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// キーとデフォルト値を入れるクラス
/// </summary>
public class AppConfigSetting<T>
{
    /// <summary>
    /// 
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required T DefaultValue { get; init; }
}
