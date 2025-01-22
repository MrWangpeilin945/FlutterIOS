using System.Text.Json.Serialization;
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
        [JsonPropertyName("staffCode")]
        public required string StaffCode { get; init; }

        /// <summary>
        /// ログインID
        /// </summary>
        [JsonPropertyName("loginId")]
        public required string LoginId { get; init; }

        /// <summary>
        /// 職員名
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        /// <summary>
        /// パスワード
        /// </summary>
        [JsonPropertyName("password")]
        public required string Password { get; init; }

        /// <summary>
        /// 有効
        /// </summary>
        [JsonPropertyName("enabled")]
        public required bool Enabled { get; init; }

        /// <summary>
        /// ロールID
        /// </summary>
        [JsonPropertyName("roleId")]
        public required Role RoleId { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        [JsonPropertyName("inputNote")]
        public required string InputNote { get; init; }

    }
}
