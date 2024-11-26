namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 機器リポジトリ
/// </summary>
public interface IEquipmentRepository
{
    /// <summary>
    /// 検査メニューを指定して検査機器リストを取得します。
    /// </summary>
    public Task<IEnumerable<Models.Equipment>> GetEquipmentAsync(int examMenuId);
}
