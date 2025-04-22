using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2001_受診者を登録する
/// </summary>
public class ExamineeUsecase : IExamineeUsecase
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IExamineeRepository _examineeRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// ユースケースを生成します。
    /// </summary>
    /// <param name="organizationRepository">団体リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="timeProvider"></param>
    public ExamineeUsecase(IOrganizationRepository organizationRepository,
                           IExamineeRepository examineeRepository,
                           TimeProvider timeProvider)
    {
        _organizationRepository = organizationRepository;
        _examineeRepository = examineeRepository;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 受診者を登録する
    /// </summary>
    /// <param name="examinees">受診者リスト</param>
    /// <returns>エラーオブジェクトリスト</returns>
    public async Task<List<ErrorObject>> StoreExamineesAsync(List<Examinee> examinees)
    {
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        // 受診者コードに紐づく受診者情報を取得する
        var existExaminees = await _examineeRepository.GetExamineeInfoAsync(
                                  examinees.Select(x => x.ExamineeCode).Distinct().ToList());

        // 団体コードに紐づく団体情報を取得する
        var organizations = await _organizationRepository.GetOrganizationInfoAsync(
                                examinees.SelectMany(x => x.Affiliations.Select(a => a.OrganizationCode)).Distinct().ToList());

        // WARNING検証
        var warningExaminees = new List<Examinee>();

        // 必須項目の空値のチェック
        var spaceCheckProperties = new[]
        {
            "ExamineeCode",     // 受診者コード
            "Name",             // 氏名
            "KanaName",         // カナ氏名
        };
        // チェックするプロパティ一覧をメソッドに渡して空値チェックする
        foreach (var warning in ValidationChecker.SpaceCheckProperties(examinees, spaceCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをExamineeにキャストしてワーニングリストに追加する
            warningExaminees.Add((Examinee)warning);
        }
        var spaceCheckChildrenProperties = new List<(string ParentProperty, string ChildProperty)>
        {
            ("Affiliations", "OrganizationCode")        // 所属->団体コード
        };
        // チェックするプロパティ一覧をメソッドに渡して空値チェックする
        foreach (var warning in ValidationChecker.SpaceCheckChildrenProperties(examinees, spaceCheckChildrenProperties, errorObjects))
        {
            // エラーのオブジェクトをExamineeにキャストしてワーニングリストに追加する
            warningExaminees.Add((Examinee)warning);
        }

        // キー重複チェック
        var duplicateCheckProperties = new[]
        {
            "ExamineeCode"      // 受診者コード
        };
        // チェックするプロパティ一覧をメソッドに渡して重複チェックする
        foreach (var warning in ValidationChecker.DuplicateCheckProperties(examinees, duplicateCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをExamineeにキャストしてワーニングリストに追加する
            warningExaminees.Add((Examinee)warning);
        }
        var duplicateCheckChildProperties = new List<(string ParentProperty, string ChildProperty)>
        {
            ("Affiliations", "OrganizationCode")        // 所属->団体コード
        };
        // チェックするプロパティ一覧をメソッドに渡して重複チェックする
        foreach (var warning in ValidationChecker.DuplicateCheckChildrenProperties(examinees, duplicateCheckChildProperties, errorObjects))
        {
            // エラーのオブジェクトをExamineeにキャストしてワーニングリストに追加する
            warningExaminees.Add((Examinee)warning);
        }

        foreach (var examinee in examinees)
        {
            foreach (var warning in examinee.Affiliations.Where(x => !organizations.Select(e => e.OrganizationCode).Contains(x.OrganizationCode)))
            {
                warningExaminees.Add(examinee);
                errorObjects.Add(new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたOrganizationCodeがシステム上に存在しません。Code:{warning.OrganizationCode}",
                    InputNote = examinee.InputNote
                });
            }
        }

        // 受診者エンティティリストを生成
        var examineeEntities = examinees.Except(warningExaminees)
                                        .Select(examinee => new ExamineeEntity
                                        {
                                            ExamineeId = existExaminees.Where(e => e.ExamineeCode == examinee.ExamineeCode)
                                                                       .Select(e => e.ExamineeId).FirstOrDefault(),
                                            ExamineeCode = examinee.ExamineeCode,
                                            Name = examinee.Name,
                                            KanaName = examinee.KanaName,
                                            Sex = (int)examinee.Sex,
                                            Birthdate = examinee.Birthdate,
                                            Affiliations = examinee.Affiliations.Select(x => new OrganizationEntity
                                            {
                                                OrganizationId = organizations.Where(o => o.OrganizationCode == x.OrganizationCode)
                                                                              .Select(o => o.OrganizationId).FirstOrDefault(),
                                                OrganizationCode = x.OrganizationCode,
                                                Name = organizations.Where(o => o.OrganizationCode == x.OrganizationCode)
                                                                    .Select(o => o.Name).FirstOrDefault() ?? ""
                                            }).ToList()
                                        }).ToList();

        // 受診者を登録する
        await _examineeRepository.UpsertExamineesAsync(examineeEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return errorObjects;
    }
}
