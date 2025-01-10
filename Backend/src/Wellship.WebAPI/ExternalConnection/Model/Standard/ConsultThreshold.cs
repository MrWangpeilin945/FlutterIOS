namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 基準値判定
    /// </summary>
    public class ConsultThreshold
    {
        /// <summary>
        /// 基準値判定コード
        /// </summary>
        public required string ThresholdCode { get; init; }

        /// <summary>
        /// 優先度
        /// </summary>
        public required int Priority { get; init; }
    }
}