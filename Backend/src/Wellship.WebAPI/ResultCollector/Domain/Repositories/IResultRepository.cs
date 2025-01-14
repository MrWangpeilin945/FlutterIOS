using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

/// <summary>
/// 検査結果リポジトリ
/// </summary>
public interface IResultRepository
{
    /// <summary>
    /// 検査結果を登録する
    /// </summary>
    public Task RegisterResultsAsync(Guid consultId, ExamResultRegisteEntity[] results);

    /// <summary>
    /// 検査結果登録時のログを記録する
    /// </summary>
    public Task WriteResultsLogAsync(Guid consultId, ExamResultRegisteEntity[] results);
}
