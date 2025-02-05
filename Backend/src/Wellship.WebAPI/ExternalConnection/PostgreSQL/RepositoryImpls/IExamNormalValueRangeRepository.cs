using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 基準値（範囲）登録リポジトリインターフェース
    /// </summary>
    public interface IExamNormalValueRangeRepository
    {
        /// <summary>
        /// 基準値（範囲）を登録する
        /// </summary>
        /// <param name="examNormalValueRangeEntities"></param>
        /// <param name="createdAt"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public Task UpsertExamNormalValueRangeAsync(List<ExamNormalValueRangeEntity> examNormalValueRangeEntities, DateTimeOffset createdAt, string createdBy);
    }
}
