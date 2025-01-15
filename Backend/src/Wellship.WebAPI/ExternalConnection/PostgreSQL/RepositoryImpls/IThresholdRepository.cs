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

        /// <summary>
        /// 存在する基準値パターン情報（基準値パターンコード、基準値パターンID）を取得する
        /// </summary>
        /// <param name="codes">基準値パターンコードのリスト</param>
        public Task<List<ThresholdEntity>> GetThresholdInfoAsync(List<string> codes);

    }
}
