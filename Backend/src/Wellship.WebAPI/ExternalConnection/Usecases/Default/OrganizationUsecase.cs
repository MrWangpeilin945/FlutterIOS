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

        // Code 必須チェック済みのリストを取得する
        var insertOrganizationsByRequiredCodes = GetCheckedRequiredCode(organizations);

        // Code 重複チェック済みのリストを取得する
        var insertOrganizationsByDuplicateCodes = GetCheckedDuplicateCode(organizations);

        var commonInsertPlaces = insertOrganizationsByRequiredCodes.Intersect(insertOrganizationsByDuplicateCodes)
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
    /// Code 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="organizations"></param>
    /// <returns></returns>
    private List<Organization> GetCheckedRequiredCode(List<Organization> organizations)
    {
        // WARNING検証
        // キー重複
        var requiredData = organizations.Where(x => string.IsNullOrWhiteSpace(x.Code));

        if (requiredData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredData);
            return organizations.Except(requiredData).ToList();
        }
        else
        {
            return new List<Organization>(organizations);
        }
    }

    /// <summary>
    /// Code 重複チェック済みのリストを取得する
    /// </summary>
    /// <param name="organizations"></param>
    /// <returns></returns>
    private List<Organization> GetCheckedDuplicateCode(List<Organization> organizations)
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
    private void AddRequiredDataErrorObjects(IEnumerable<Organization> requiredData)
    {
        var errorObjects = requiredData
            .Select(r => new ErrorObject
            {
                Code = "10004",
                Message = "必須項目が不足しています。Code",
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
