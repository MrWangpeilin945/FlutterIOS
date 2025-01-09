using System.Transactions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
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

        /// <summary>
        /// ユースケースを生成します。
        /// </summary>
        /// <param name="dbConnectionProvider">dbConnectionProvider</param>
        /// <param name="organizationRepository">団体リポジトリ</param>
        /// <param name="examineeRepository">受診者リポジトリ</param>
        /// <param name="affiliationRepository">所属リポジトリ</param>
        public ExamineeUsecase(IDbConnectionProvider dbConnectionProvider, IOrganizationRepository organizationRepository, IExamineeRepository examineeRepository, IAffiliationRepository affiliationRepository)
        {
            _dbConnectionProvider = dbConnectionProvider;
            _organizationRepository = organizationRepository;
            _examineeRepository = examineeRepository;
            _affiliationRepository = affiliationRepository;
            _errorObjects = new List<ErrorObject>();
        }

        /// <summary>
        /// 受診者を登録する
        /// </summary>
        /// <param name="examinees">受診者リスト</param>
        /// <returns>エラーオブジェクトリスト</returns>
        public async Task<List<ErrorObject>> StoreExamineesAsync(List<Examinee> examinees)
        {
            // WARNING検証後、登録対象受診者リスト取得
            var insertExaminees = await GetCheckedExaminees(examinees);

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

                    DateTime createdAt = DateTime.Now;
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
        private async Task<List<Examinee>> GetCheckedExaminees(List<Examinee> examinees)
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
    }
}
