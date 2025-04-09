namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 受診者リポジトリ
/// </summary>
public interface IExamineeRepository
{
    /// <summary>
    /// 受診者IDで受診者を取得します。
    /// </summary>
    /// <param name="examineeId">受診者ID</param>
    public Task<Models.Examinee> GetExamineeAsync(Guid examineeId);

    /// <summary>
    /// 受診者ID配列で受診者を取得します。
    /// </summary>
    /// <param name="consultExamineeIds">受診者ID配列</param>
    public Task<Models.ConsultExaminee[]> GetConsultExamineesAsync(Guid[] consultExamineeIds);
}
