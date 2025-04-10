using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2008_団体を登録する
/// </summary>
public class OrganizationUsecase : IOrganizationUsecase
{
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
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 団体を登録する。
    /// </summary>
    /// <param name="organizations">団体</param>
    /// <returns></returns>
    public async Task<List<ErrorObject>> StoreOrganizationsAsync(List<Organization> organizations)
    {
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        // WARNING検証
        var warningOrganizations = new List<Organization>();

        // 必須項目の空値のチェック
        var spaceCheckProperties = new[]
        {
            "Code",     // 団体コード
            "Name"      // 団体名
        };
        // チェックするプロパティ一覧をメソッドに渡してチェックエラーのconsultを取得する
        foreach (var warning in ValidationChecker.SpaceCheckProperties(organizations, spaceCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをOrganizationにキャストしてワーニングリストに追加する
            if (warning is Organization organization)
            {
                warningOrganizations.Add(organization);
            }
        }

        // キー重複チェック
        var duplicateCheckProperties = new[]
        {
            "Code"      // 団体コード
        };
        // チェックするプロパティ一覧をメソッドに渡して重複チェックを取得する
        foreach (var warning in ValidationChecker.DuplicateCheckProperties(organizations, duplicateCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをOrganizationにキャストしてワーニングリストに追加する
            if (warning is Organization organization)
            {
                warningOrganizations.Add(organization);
            }
        }

        // 団体エンティティリスト生成
        var organizationEntities = organizations.Except(warningOrganizations)
                                                .Select(item => new OrganizationEntity
                                                {
                                                    OrganizationCode = item.Code,
                                                    Name = item.Name
                                                }).ToList();

        // 団体登録
        await _organizationRepository.UpsertOrganizationsAsync(organizationEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return errorObjects;
    }
}
