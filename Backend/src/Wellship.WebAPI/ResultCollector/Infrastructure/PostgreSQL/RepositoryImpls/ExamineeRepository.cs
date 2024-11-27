
using Dapper;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.RepositoryImpls;

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
    /// 受診者IDで受診者を取得します。
    /// </summary>
    /// <param name="examineeId">ログインID</param>
    public async Task<Examinee> GetExamineeAsync(int examineeId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
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
            examinee_id = @ExamineeId;";

        var examinee = await connection.QuerySingleAsync<ExamineeEntity>(sql, new { ExamineeId = examineeId });

        if (examinee is null)
        {
            throw new ResourceNotFoundException($"受診者が存在しません。ID: {examineeId}");
        }

        return new Examinee()
        {
            ExamineeId = examinee.ExamineeId,
            ExamineeCode = examinee.ExamineeCode,
            Name = examinee.Name,
            KanaName = examinee.KanaName,
            Sex = (Sex)examinee.Sex,
            Birthdate = new Birthdate(DateOnly.FromDateTime(examinee.Birthdate))
        };
    }
}
