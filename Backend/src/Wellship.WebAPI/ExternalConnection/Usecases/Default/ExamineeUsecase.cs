using System.Transactions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2001_受診者を登録する
/// </summary>
public class ExamineeUsecase : IExamineeUsecase
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly List<ErrorObject> _errorObjects;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IExamineeRepository _examineeRepository;
    private readonly IAffiliationRepository _affiliationRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// ユースケースを生成します。
    /// </summary>
    /// <param name="dbConnectionProvider">dbConnectionProvider</param>
    /// <param name="organizationRepository">団体リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="affiliationRepository">所属リポジトリ</param>
    /// <param name="timeProvider"></param>
    public ExamineeUsecase(IDbConnectionProvider dbConnectionProvider, IOrganizationRepository organizationRepository,
                           IExamineeRepository examineeRepository, IAffiliationRepository affiliationRepository,
                           TimeProvider timeProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _organizationRepository = organizationRepository;
        _examineeRepository = examineeRepository;
        _affiliationRepository = affiliationRepository;
        _errorObjects = new List<ErrorObject>();
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 受診者を登録する
    /// </summary>
    /// <param name="examinees">受診者リスト</param>
    /// <returns>エラーオブジェクトリスト</returns>
    public async Task<List<ErrorObject>> StoreExamineesAsync(List<Examinee> examinees)
    {
        // 必須チェック済みのリストを取得する
        var insertExamineesByRequired = GetCheckedRequired(examinees);

        // キー重複の確認
        var insertExamineesByDuplicated = GetCheckedDuplicateKey(examinees);

        // 団体の確認
        var insertExamineesByOrganizations = await GetCheckedOrganizations(examinees);

        var insertExaminees = insertExamineesByRequired.Intersect(insertExamineesByRequired)
                                                       .Intersect(insertExamineesByDuplicated)
                                                       .Intersect(insertExamineesByOrganizations)
                                                       .ToList();

        // 受診者エンティティリストを生成
        var examineeEntities = insertExaminees.Select(examinee => new ExamineeEntity
        {
            ExamineeId = Guid.NewGuid(),
            ExamineeCode = examinee.ExamineeCode,
            Name = examinee.Name,
            KanaName = examinee.KanaName,
            Sex = (int)examinee.Sex,
            Birthdate = examinee.Birthdate
        }).ToList();

        using var scope = TransactionScopeHelper.GetTransactionScope();
        {
            // トランザクション登録
            using var connection = await _dbConnectionProvider.GetOrOpenAsync();
            {
                connection.EnlistTransaction(Transaction.Current);

                var createdAt = _timeProvider.GetUtcNow();
                string createdBy = "ExternalConnection";

                // 受診者を登録する
                await _examineeRepository.UpsertExamineesAsync(examineeEntities, createdAt, createdBy);

                // 所属を登録する
                await _affiliationRepository.InsertAffiliationsAsync(insertExaminees, createdAt, createdBy);

                // コミット
                scope.Complete();
            }
        }

        return _errorObjects;
    }

    /// <summary>
    /// 団体チェック処理済の受診者リストを取得する。
    /// </summary>
    /// <param name="examinees">受診者リスト</param>
    /// <returns>受診者リスト</returns>
    private async Task<List<Examinee>> GetCheckedOrganizations(List<Examinee> examinees)
    {
        var results = new List<Examinee>();

        // WARINING検証
        var errorOrganizationCodes = await OrganizationCodesExists(examinees);

        // 存在しない団体コードが指定されている場合
        if (errorOrganizationCodes.Count > 0)
        {
            // 返却用エラーオブジェクトに追加
            AddErrorObjects(examinees, errorOrganizationCodes);
            // 団体コードが存在する受診者のみ抽出
            results = examinees
                .Where(examinee => !examinee.Affiliations.Any(affiliation => errorOrganizationCodes.Contains(affiliation.OrganizationCode)))
                .ToList();
        }
        else
        {
            results = new List<Examinee>(examinees);
        }

        return results;
    }

    /// <summary>
    /// 団体の存在検証
    /// </summary>
    /// <param name="examinees">受診者リスト</param>
    /// <returns>存在しない団体コードリスト</returns>
    private async Task<List<string>> OrganizationCodesExists(List<Examinee> examinees)
    {
        var results = new List<string>();

        // 団体コードを一意に抽出
        var organizationCodes = examinees
            .SelectMany(examinee => examinee.Affiliations)
            .Select(affiliation => affiliation.OrganizationCode)
            .Distinct()
            .ToList();

        // 団体コードを基に団体情報を取得する
        var existOrganizationCodes = await _organizationRepository.GetOrganizationsByCodesAsync(organizationCodes);

        // 存在しない団体コードを取得する
        results = organizationCodes.Except(existOrganizationCodes).ToList();

        return results;
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する
    /// </summary>
    /// <param name="examinees">受診者リスト</param>
    /// <param name="errorOrganizationCodes">団体取得に失敗した団体コードリスト</param>
    private void AddErrorObjects(IEnumerable<Examinee> examinees, List<string> errorOrganizationCodes)
    {
        // 団体が取得できないエラーを返却用エラーオブジェクトに追加
        var errorObjects = examinees
            .SelectMany(examinee => examinee.Affiliations
            .Where(affiliation => errorOrganizationCodes.Contains(affiliation.OrganizationCode))
            .Select(affiliation => new ErrorObject
            {
                Code = "10001",
                Message = $"指定されたOrganizationCodeがシステム上に存在しません。Code:{affiliation.OrganizationCode}",
                InputNote = examinee.InputNote
            })).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="examinees"></param>
    /// <returns></returns>
    private List<Examinee> GetCheckedRequired(List<Examinee> examinees)
    {
        // WARNING検証
        // 未入力
        var requiredExamineeCodeData = examinees.Where(x => string.IsNullOrWhiteSpace(x.ExamineeCode));
        if (requiredExamineeCodeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredExamineeCodeData, "ExamineeCode");
        }

        // 未入力
        var requiredNameData = examinees.Where(x => string.IsNullOrWhiteSpace(x.Name));
        if (requiredNameData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredNameData, "Name");
        }

        // 未入力
        var requiredKanaNameData = examinees.Where(x => string.IsNullOrWhiteSpace(x.KanaName));
        if (requiredKanaNameData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredKanaNameData, "KanaName");
        }

        // 未入力
        var requiredOrganizationCodeData = examinees.Where(x => x.Affiliations
                                                                 .Any(a => string.IsNullOrWhiteSpace(a.OrganizationCode)));
        if (requiredOrganizationCodeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredOrganizationCodeData, "Affiliations.OrganizationCode");
        }

        return examinees.Except(requiredExamineeCodeData)
                        .Except(requiredNameData)
                        .Except(requiredKanaNameData)
                        .Except(requiredOrganizationCodeData)
                        .ToList();
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(必須項目エラー）
    /// </summary>
    /// <param name="requiredData"></param>
    /// <param name="itemName"></param>
    private void AddRequiredDataErrorObjects(IEnumerable<Examinee> requiredData, string itemName)
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
    /// キー重複の確認
    /// </summary>
    /// <param name="examinees"></param>
    /// <returns></returns>
    private List<Examinee> GetCheckedDuplicateKey(List<Examinee> examinees)
    {
        // キー重複
        var duplicateExamineeCodes = examinees.GroupBy(x => x.ExamineeCode)
                                              .Where(x => x.Count() > 1)
                                              .SelectMany(x => x);
        if (duplicateExamineeCodes.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddDuplicateExamineeCodeErrorObjects(duplicateExamineeCodes.ToList());
        }

        // キー重複
        var duplicateOrganizationCodes = examinees.Where(x => x.Affiliations
                                                               .GroupBy(a => a.OrganizationCode)
                                                               .Any(g => g.Count() > 1));
        if (duplicateOrganizationCodes.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddDuplicateOrganizationCodeErrorObjects(duplicateOrganizationCodes.ToList());
        }

        return examinees.Except(duplicateExamineeCodes)
                        .Except(duplicateOrganizationCodes)
                        .ToList();
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(受診者コード重複エラー）
    /// </summary>
    /// <param name="duplicatedData"></param>
    private void AddDuplicateExamineeCodeErrorObjects(IEnumerable<Examinee> duplicatedData)
    {
        var errorObjects = duplicatedData
            .Select(d => new ErrorObject
            {
                Code = "10003",
                Message = $"キー項目が重複しています。ExamineeCode:{d.ExamineeCode}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(所属重複エラー）
    /// </summary>
    /// <param name="duplicatedData"></param>
    private void AddDuplicateOrganizationCodeErrorObjects(IEnumerable<Examinee> duplicatedData)
    {
        // OrganizationCodeが重複したレコードをExamineeCode単位にマージする        
        var duplicateWithExamineeCode = duplicatedData
                                            .SelectMany(x => x.Affiliations.Select(a => new { x.ExamineeCode, x.InputNote, a.OrganizationCode }))
                                            .GroupBy(x => new { x.ExamineeCode, x.InputNote, x.OrganizationCode })
                                            .Where(x => x.Count() > 1)
                                            .Select(x => new { x.Key.ExamineeCode, x.Key.InputNote, x.Key.OrganizationCode })
                                            .Distinct();
        var errorObjects = duplicateWithExamineeCode
            .Select(d => new ErrorObject
            {
                Code = "10003",
                Message = $"キー項目が重複しています。Affiliations.OrganizationCode:{d.OrganizationCode}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }
}
