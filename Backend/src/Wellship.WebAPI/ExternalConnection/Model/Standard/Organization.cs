namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 団体
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// 班コード
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// 班名
        /// </summary>
        public required string Name { get; init; }
    }
}
