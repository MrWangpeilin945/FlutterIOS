using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 基準パターンを登録するRepository層
    /// </summary>
    public interface IThresholdRepository
    {
        /// <summary>
        /// 基準パターンを登録するRepository層
        /// </summary>
        /// <param name="thresholds">基準パターン</param>
        /// <param name="createdAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        public Task UpsertThresholdsAsync(List<ThresholdEntity> thresholds, DateTime createdAt, string createdBy);

    }
}
