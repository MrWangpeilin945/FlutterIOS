using Ryobi.Wellship.Core.Enums;
namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 職員
    /// </summary>
    public class Staff
    {
        /// <summary>
        /// 職員コード
        /// </summary>
        public required string StaffCode { get; init; }

        /// <summary>
        /// ログインID
        /// </summary>
        public required string LoginId { get; init; }

        /// <summary>
        /// 職員名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// パスワード
        /// </summary>
        public required string Password { get; init; }

        /// <summary>
        /// 有効
        /// </summary>
        public required bool Enabled { get; init; }

        /// <summary>
        /// ロールID
        /// </summary>
        public required Role RoleId { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }

    }
}
