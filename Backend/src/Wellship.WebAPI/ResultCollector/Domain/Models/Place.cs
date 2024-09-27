namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 会場
/// </summary>
public class Place
{

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Place(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// 会場ID
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 会場名
    /// </summary>
    public string Name { get; }
}