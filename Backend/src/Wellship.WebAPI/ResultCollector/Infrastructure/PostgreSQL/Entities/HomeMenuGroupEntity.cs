namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

/// <summary>
/// ホームメニューグループのDapperマッピング用エンティティ
/// </summary>
public class HomeMenuGroupEntity
{
    /// <summary>
    /// グループID
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// グループ名
    /// </summary>
    public string GroupName { get; set; } = null!;

    /// <summary>
    /// グループの並び順
    /// </summary>
    public int GroupOrderNumber { get; set; }

    /// <summary>
    /// メニューID
    /// </summary>
    public int MenuId { get; set; }

    /// <summary>
    /// メニューの並び順
    /// </summary>
    public int MenuOrderNumber { get; set; }

    /// <summary>
    /// メニュー名
    /// </summary>
    public string MenuName { get; set; } = null!;

    /// <summary>
    /// パス
    /// </summary>
    public string Path { get; set; } = null!;
}
