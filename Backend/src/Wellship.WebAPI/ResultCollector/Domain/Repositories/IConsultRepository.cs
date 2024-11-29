using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

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
    public Task<bool> ConsultExistsAsync(string consultNumber);

    /// <summary>
    /// 受診を取得します。
    /// </summary>
    public Task<Consult> GetConsultAsync(string consultNumber);

    /// <summary>
    /// 受診リストを取得します。
    /// </summary>
    public Task<IEnumerable<Consult>> GetConsultsAsync(string[] consultNumbers);

    /// <summary>
    /// 受診番号を指定して未受診の検査項目を取得します。
    /// </summary>
    public Task<UnexaminedConsult> GetUnexaminedConsultAsync(string consultNumber);

    /// <summary>
    /// 未受診の検査項目を受診単位のリストで取得します。
    /// </summary>
    public Task<IEnumerable<UnexaminedConsult>> GetUnexaminedConsultsAsync(string[] consultNumbers);
}
