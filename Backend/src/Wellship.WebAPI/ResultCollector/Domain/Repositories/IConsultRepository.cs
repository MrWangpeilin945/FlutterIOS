namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 受診リポジトリ
/// </summary>
public interface IConsultRepository
{
    /// <summary>
    /// 受診が存在するか
    /// </summary>
    /// <param name="consultNumber">受診番号</param>
    public bool ConsultExists(string consultNumber);
}
