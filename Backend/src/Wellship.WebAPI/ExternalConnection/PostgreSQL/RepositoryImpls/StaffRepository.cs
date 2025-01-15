using Dapper;
using System.Data.Common;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 職員リポジトリ
    /// </summary>
    public class StaffRepository : IStaffRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// リポジトリの生成
        /// </summary>
        /// <param name="dbConnectionProvider"></param>
        public StaffRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// 職員を登録する
        /// </summary>
        /// <param name="staffEntities"></param>
        /// <param name="createdAt"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public async Task UpsertStaffAsync(List<StaffEntity> staffEntities, DateTime createdAt, string createdBy)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var transaction = await connection.BeginTransactionAsync();
            try
            {
                var upsertItems = staffEntities.Select(s => new
                {
                    StaffCode = s.StaffCode,
                    LoginId = s.LoginId,
                    Name = s.Name,
                    PasswordHash = s.PasswordHash,
                    PasswordSalt = s.PasswordSalt,
                    Enabled = s.Enabled,
                    RoleId = (int)s.RoleId,
                    CreatedAt = createdAt,
                    CreatedBy = createdBy
                }).ToArray();

                // Upsert文を実行
                string mergeSql = @"
                        merge
                        into resultcollector.staffs as s
                            using (values (@StaffCode, @LoginId, @Name, @PasswordHash, @PasswordSalt, @Enabled, @RoleId, @CreatedAt, @CreatedBy)) as new_data (
                                staff_code,
                                login_id,
                                name,
                                password_hash,
                                password_salt,
                                enabled,
                                role_id,
                                created_at,
                                created_by
                            )
                                on s.staff_code = new_data.staff_code
                        when matched then update
                        set
                            login_id = new_data.login_id,
                            name = new_data.name,
                            password_hash = new_data.password_hash,
                            password_salt = new_data.password_salt,
                            enabled = new_data.enabled,
                            role_id = new_data.role_id,
                            created_at = new_data.created_at,
                            created_by = new_data.created_by when not matched then
                        insert (
                            staff_code,
                            login_id,
                            name,
                            password_hash,
                            password_salt,
                            enabled,
                            role_id,
                            created_at,
                            created_by
                        )
                        values (
                            new_data.staff_code,
                            new_data.login_id,
                            new_data.name,
                            new_data.password_hash,
                            new_data.password_salt,
                            new_data.enabled,
                            new_data.role_id,
                            new_data.created_at,
                            new_data.created_by
                        );";

                await connection.ExecuteAsync(mergeSql, upsertItems);
                await transaction.CommitAsync();
            }
            catch (DbException e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// 職員コードとログインIDのペアを取得する
        /// </summary>
        /// <param name="staffCodesAndLoginIds"></param>
        /// <returns></returns>
        public async Task<List<(string staffCode, string loginId)>> GetStaffsByLoginIdsAsync(List<(string staffCode, string loginId)> staffCodesAndLoginIds)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();

            // パラメータを準備
            var sqlBuilder = new List<string>();
            var parameters = new DynamicParameters();

            // 各ペアを動的に処理
            for (int i = 0; i < staffCodesAndLoginIds.Count; i++)
            {
                var loginIdParam = $"LoginId{i}";
                var staffCodeParam = $"StaffCode{i}";

                // 条件を動的に追加
                sqlBuilder.Add($"(login_id = @{loginIdParam} AND staff_code <> @{staffCodeParam})");

                // パラメータを追加
                parameters.Add(loginIdParam, staffCodesAndLoginIds[i].loginId);
                parameters.Add(staffCodeParam, staffCodesAndLoginIds[i].staffCode);
            }

            // 動的に条件を OR で結合
            var conditions = string.Join(" OR ", sqlBuilder);

            // クエリ構築
            var sql = $@"
                SELECT
                    staff_code, login_id
                FROM
                    resultcollector.staffs
                WHERE
                    {conditions};";

            // クエリ実行
            var result = await connection.QueryAsync<(string staffCode, string loginId)>(sql, parameters);
            return result.ToList();
        }
    }
}
