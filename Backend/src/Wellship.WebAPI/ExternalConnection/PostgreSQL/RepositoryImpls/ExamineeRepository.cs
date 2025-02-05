using System.Transactions;

using Dapper;

using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Helper;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 受診者リポジトリ
    /// </summary>
    public class ExamineeRepository : IExamineeRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>
        /// リポジトリを生成します。
        /// </summary>
        /// <param name="dbConnectionProvider">dbConnectionProvider</param>
        public ExamineeRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        /// <summary>
        /// 受診者を登録する。
        /// </summary>
        /// <param name="examineeEntities">受診者エンティティリスト</param>
        /// <param name="createdAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        /// <returns></returns>
        public async Task UpsertExamineesAsync(List<ExamineeEntity> examineeEntities, DateTimeOffset createdAt, string createdBy)
        {
            using var connection = await _dbConnectionProvider.GetOrOpenAsync();
            {
                using var scope = TransactionScopeHelper.GetTransactionScope();
                {
                    // トランザクションを接続に登録する
                    connection.EnlistTransaction(Transaction.Current);

                    string createTempTableSql = @"
                        create temp table temp_examinees ( 
                            examinee_id uuid not null,
                            examinee_code text not null,
                            name text not null,
                            kana_name text not null,
                            sex integer not null,
                            birthdate date not null
                        ) on commit drop;";
                    await connection.ExecuteAsync(createTempTableSql);

                    // 一時テーブルに挿入
                    await BulkInsertHelper.BulkInsert(examineeEntities,
                        examinee =>
                        $"(" +
                        $"{SqlFormatter.EscapeSqlValue(examinee.ExamineeId)}," +
                        $"{SqlFormatter.EscapeSqlValue(examinee.ExamineeCode)}, " +
                        $"{SqlFormatter.EscapeSqlValue(examinee.Name)}, " +
                        $"{SqlFormatter.EscapeSqlValue(examinee.KanaName)}, " +
                        $"{SqlFormatter.EscapeSqlValue(examinee.Sex)}, " +
                        $"{SqlFormatter.EscapeSqlValue(examinee.Birthdate)}" +
                        $")",
                        "temp_examinees",
                        connection);

                    // Upsert文を実行
                    string upsertSql = @"
                        insert into resultcollector.examinees (examinee_id, examinee_code, name, kana_name, sex, birthdate, created_at, created_by)
                        select
                            examinee_id,
                            examinee_code,
                            name,
                            kana_name,
                            sex,
                            birthdate,
                            @CreatedAt,
                            @CreatedBy
                        from
                            temp_examinees
                        on conflict (examinee_code)
                        do update set
                            name = excluded.name,
                            kana_name = excluded.kana_name,
                            sex = excluded.sex,
                            birthdate = excluded.birthdate,
                            created_at = excluded.created_at,
                            created_by = excluded.created_by;
                    ";
                    await connection.ExecuteAsync(upsertSql, new { CreatedAt = createdAt, CreatedBy = createdBy });

                    // コミット
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// 受診者情報を取得する
        /// </summary>
        /// <param name="examinees">受診者コードのリスト</param>
        public async Task<List<ExamineeEntity>> GetExamineeInfoAsync(List<string> examinees)
        {
            var connection = await _dbConnectionProvider.GetOrOpenAsync();
            var sql = @"
                    select
                        examinee_id as ExamineeId
                        , examinee_code as ExamineeCode
                        , name as Name
                        , kana_name as KanaName
                        , sex as Sex
                        , birthdate as Birthdate
                    from
                        resultcollector.examinees
                    where
                        examinee_code = any(@ExamineeCodes);";

            var result = await connection.QueryAsync<ExamineeEntity>(sql, new { ExamineeCodes = examinees });

            return result.ToList();
        }
    }
}
