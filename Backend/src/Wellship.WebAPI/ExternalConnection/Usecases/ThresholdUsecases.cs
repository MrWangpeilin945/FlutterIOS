
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// 基準パターンを登録するUsecase層
    /// </summary>
    public class ThresholdUsecases : IThresholdUsecases
    {
        private readonly List<ErrorObject> _errorObjects;
        private readonly IThresholdRepository _thresholdRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="thresholdRepository"></param>
        public ThresholdUsecases(IThresholdRepository thresholdRepository)
        {
            _thresholdRepository = thresholdRepository;
            _errorObjects = new List<ErrorObject>();
        }

        /// <summary>
        /// EC2014_基準パターンを登録する
        /// </summary>
        /// <param name="thresholds">基準パターン</param>
        /// <returns>エラーリスト</returns>
        public async Task<List<ErrorObject>> StoreThresholdsAsync(List<Threshold> thresholds)
        {
            // エンティティリスト生成
            List<ThresholdEntity> thresholdEntities = thresholds.Select(item =>new ThresholdEntity
            {
                ThresholdCode = item.Code,
                Name = item.Name
            }).ToList();

            // Repository処理
            await _thresholdRepository.UpsertThresholdsAsync(thresholdEntities, DateTime.Now, "ExternalConnection");

            return _errorObjects;
        }
    }

}
