using Dapper;

using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;

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
        using var scope = TransactionScopeHelper.GetTransactionScope();
        {
            using var connection = await _dbConnectionProvider.GetOrOpenAsync();
            {
                var upsertExaminees = examineeEntities.Select((x, index) => new
                {
                    ExamineeId = Guid.NewGuid(),
                    ExamineeCode = x.ExamineeCode,
                    Name = x.Name,
                    KanaName = x.KanaName,
                    Sex = (int)x.Sex,
                    Birthdate = x.Birthdate,
                    CreatedAt = createdAt,
                    CreatedBy = createdBy,
                }).ToArray();

                var examineeIds = examineeEntities.Select(x => x.ExamineeId).ToList();
                // affiliations（所属）の削除
                const string deleteAffiliationsSql = @"
                delete from 
                    resultcollector.affiliations
                where
                    examinee_id = any (@ExamineeIds);";
                await connection.ExecuteAsync(deleteAffiliationsSql, new { ExamineeIds = examineeIds });

                // examinees（受診者）
                const string mergeExamineesSql = @"
                merge
                into resultcollector.examinees as examinee
                    using (values (@ExamineeId, @ExamineeCode, @Name, @KanaName, @Sex, @Birthdate,
                                   @CreatedAt, @CreatedBy)) as new_data(
                        examinee_id
                        , examinee_code
                        , name
                        , kana_name
                        , sex
                        , birthdate
                        , created_at
                        , created_by
                    )
                    on examinee.examinee_code = new_data.examinee_code
                when matched then 
                    update set
                        name = new_data.name
                        , kana_name = new_data.kana_name
                        , sex = new_data.sex
                        , birthdate = new_data.birthdate
                        , created_at = new_data.created_at
                        , created_by = new_data.created_by 
                when not matched then
                    insert (
                        examinee_id
                        , examinee_code
                        , name
                        , kana_name
                        , sex
                        , birthdate
                        , created_at
                        , created_by
                    )
                    values (
                        new_data.examinee_id
                        , new_data.examinee_code
                        , new_data.name
                        , new_data.kana_name
                        , new_data.sex
                        , new_data.birthdate
                        , new_data.created_at
                        , new_data.created_by
                    );";
                await connection.ExecuteAsync(mergeExamineesSql, upsertExaminees);

                // affiliations（所属）の登録
                var examineeCodes = examineeEntities.Select(x => x.ExamineeCode).ToList();
                const string selectExamineeSQL = @"
                select
                    examinee_id as ExamineeId
                    , examinee_code as ExamineeCode
                from 
                    resultcollector.examinees 
                where 
                    examinee_code = any (@ExamineeCodes);";
                var examinees = await connection.QueryAsync<ExamineeEntity>(selectExamineeSQL, new { ExamineeCodes = examineeCodes });
                var insertAffiliations = examineeEntities.SelectMany(x => x.Affiliations.Select(a => new
                {
                    ExamineeId = examinees.Where(e => e.ExamineeCode.Equals(x.ExamineeCode))
                                          .Select(e => e.ExamineeId).FirstOrDefault(),
                    OrganizationId = a.OrganizationId,
                    CreatedAt = createdAt,
                    CreatedBy = createdBy
                }));
                const string insertAffiliationsSql = @"
                insert into resultcollector.affiliations 
                (
                    examinee_id
                    , organization_id
                    , created_at
                    , created_by
                ) 
                values
                (
                    @ExamineeId
                    , @OrganizationId
                    , @CreatedAt
                    , @CreatedBy
                );";
                await connection.ExecuteAsync(insertAffiliationsSql, insertAffiliations);
            }
            scope.Complete();
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
