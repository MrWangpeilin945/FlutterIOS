namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 班
/// </summary>
public class Team
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Team(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// 班ID
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 班名
    /// </summary>
    public string Name { get; }
}