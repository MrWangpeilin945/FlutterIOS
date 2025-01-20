namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities
{
    /// <summary>
    /// 職員エンティティ
    /// </summary>
    public class StaffEntity
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
        /// パスワードハッシュ
        /// </summary>
        public required byte[] PasswordHash { get; init; }
        /// <summary>
        /// パスワードソルト
        /// </summary>
        public required byte[] PasswordSalt { get; init; }
        /// <summary>
        /// 有効
        /// </summary>
        public required bool Enabled { get; init; }
        /// <summary>
        /// ロールID
        /// </summary>
        public required int RoleId { get; init; }
    }
}
