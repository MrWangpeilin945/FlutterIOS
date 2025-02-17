using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2008_団体を登録する
/// </summary>
public class OrganizationUsecase : IOrganizationUsecase
{
    private readonly List<ErrorObject> _errorObjects;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="organizationRepository">団体リポジトリ</param>
    /// <param name="timeProvider"></param>
    public OrganizationUsecase(IOrganizationRepository organizationRepository, TimeProvider timeProvider)
    {
        _organizationRepository = organizationRepository;
        _errorObjects = new List<ErrorObject>();
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 団体を登録する。
    /// </summary>
    /// <param name="organizations">団体</param>
    /// <returns></returns>
    public async Task<List<ErrorObject>> StoreOrganizationsAsync(List<Organization> organizations)
    {
        // 団体エンティティリスト生成
        var organizationEntities = organizations.Select(item => new OrganizationEntity
        {
            OrganizationCode = item.Code,
            Name = item.Name
        }).ToList();

        // 団体登録
        await _organizationRepository.UpsertOrganizationsAsync(organizationEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return _errorObjects;
    }
}
