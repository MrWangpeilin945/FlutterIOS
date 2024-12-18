
using Dapper;
using Namotion.Reflection;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

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
    public async Task<Examinee> GetExamineeAsync(Guid examineeId)
    {
        var connection = await _dbConnectionProvider.GetOrOpenAsync();
        const string sql = @"
        select
            e.examinee_id as ExamineeId
            , e.examinee_code as ExamineeCode
            , e.name as Name
            , e.kana_name as KanaName
            , e.sex as Sex
            , e.birthdate as Birthdate 
            , a.organization_id as OrganizationId
            , o.organization_code as OrganizationCode
            , o.name as OrganizationName
            , o.order_number as OrderNumber
        from
            resultcollector.examinees e
            left join resultcollector.affiliations a
                on e.examinee_id = a.examinee_id
            left join resultcollector.organizations o
                on a.organization_id = o.organization_id
        where
            e.examinee_id = @ExamineeId;";

        var examinee = await connection.QueryAsync<ExamineeEntity>(sql, new { ExamineeId = examineeId });

        if (examinee is null)
        {
            throw new ResourceNotFoundException($"受診者が存在しません。ID: {examineeId}");
        }

        return new Examinee()
        {
            ExamineeId = examinee.First().ExamineeId,
            ExamineeCode = examinee.First().ExamineeCode,
            Name = examinee.First().Name,
            KanaName = examinee.First().KanaName,
            Sex = (Sex)examinee.First().Sex,
            Birthdate = new Birthdate(DateOnly.FromDateTime(examinee.First().Birthdate)),
            Affiliations = string.IsNullOrWhiteSpace(examinee.First().OrganizationCode) ? []
                            : examinee.Select(x => new Affiliations
                                        {
                                            OrganizationId = x.OrganizationId,
                                            OrganizationCode = x.OrganizationCode,
                                            OrganizationName = x.OrganizationName,
                                            OrderNumber = x.OrderNumber
                                        })
        };
    }
}
