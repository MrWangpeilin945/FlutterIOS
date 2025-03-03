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
        _errorObjects.Clear();

        // 必須チェック済みのリストを取得する
        var insertOrganizationsByRequired = GetCheckedRequired(organizations);

        // 重複チェック済みのリストを取得する
        var insertOrganizationsByDuplicated = GetCheckedDuplicated(organizations);

        var commonInsertPlaces = insertOrganizationsByRequired.Intersect(insertOrganizationsByDuplicated)
                                                              .ToList();

        // 団体エンティティリスト生成
        var organizationEntities = commonInsertPlaces.Select(item => new OrganizationEntity
        {
            OrganizationCode = item.Code,
            Name = item.Name
        }).ToList();

        // 団体登録
        await _organizationRepository.UpsertOrganizationsAsync(organizationEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return _errorObjects;
    }


    /// <summary>
    /// 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="organizations"></param>
    /// <returns></returns>
    private List<Organization> GetCheckedRequired(List<Organization> organizations)
    {
        // WARNING検証
        // 未入力(Code)
        var requiredCodeData = organizations.Where(x => string.IsNullOrWhiteSpace(x.Code));
        if (requiredCodeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredCodeData, "Code");
        }

        // 未入力(Name)
        var requiredNameData = organizations.Where(x => string.IsNullOrWhiteSpace(x.Name));
        if (requiredNameData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredNameData, "Name");
        }

        return organizations.Except(requiredCodeData)
                            .Except(requiredNameData)
                            .ToList();
    }

    /// <summary>
    /// 重複チェック済みのリストを取得する
    /// </summary>
    /// <param name="organizations"></param>
    /// <returns></returns>
    private List<Organization> GetCheckedDuplicated(List<Organization> organizations)
    {
        // WARNING検証
        // キー重複
        var duplicatedTeamCodes = organizations.GroupBy(x => x.Code).Where(x => x.Count() > 1).Select(x => x.Key).ToHashSet();

        if (duplicatedTeamCodes.Any())
        {
            var duplicatedData = organizations.Where(x => duplicatedTeamCodes.Contains(x.Code));

            // 返却用エラーオブジェクトに追加
            AddDuplicateDataErrorObjects(duplicatedData);
            return organizations.Except(duplicatedData).ToList();
        }
        else
        {
            return new List<Organization>(organizations);
        }
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(必須項目エラー）
    /// </summary>
    /// <param name="requiredData"></param>
    private void AddRequiredDataErrorObjects(IEnumerable<Organization> requiredData, string itemName)
    {
        var errorObjects = requiredData
            .Select(r => new ErrorObject
            {
                Code = "10004",
                Message = $"必須項目が不足しています。{itemName}",
                InputNote = r.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(キーが重複するレコード）
    /// </summary>
    /// <param name="duplicatedData"></param>
    private void AddDuplicateDataErrorObjects(IEnumerable<Organization> duplicatedData)
    {
        var errorObjects = duplicatedData
            .Select(d => new ErrorObject
            {
                Code = "10003",
                Message = $"キー項目が重複しています。Code:{d.Code}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }
}
