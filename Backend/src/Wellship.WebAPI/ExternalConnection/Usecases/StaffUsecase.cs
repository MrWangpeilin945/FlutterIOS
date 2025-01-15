using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{

    /// <summary>
    /// 職員を登録する
    /// </summary>
    public class StaffUsecase : IStaffUsecase
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;
        private readonly List<ErrorObject> _errorObjects;
        private readonly IStaffRepository _staffRepository;

        /// <summary>
        /// ユースケースを作成する
        /// </summary>
        /// <param name="dbConnectionProvider"></param>
        /// <param name="staffRepository"></param>
        public StaffUsecase(IDbConnectionProvider dbConnectionProvider, IStaffRepository staffRepository)
        {
            _dbConnectionProvider = dbConnectionProvider;
            _errorObjects = new List<ErrorObject>();
            _staffRepository = staffRepository;
        }

        /// <summary>
        /// 職員を登録する
        /// </summary>
        /// <param name="staffs"></param>
        /// <returns></returns>
        public async Task<List<ErrorObject>> StroreStaffsAsync(List<Model.Standard.Staff> staffs)
        {
            // ログインIDの確認
            var insertStaffs = await GetCheckedStaffs(staffs);

            List<PostgreSQL.Entities.StaffEntity> staffEntities = insertStaffs.Select(s =>
            {
                // パスワードからハッシュとソルトを取得
                Password password = Password.Create(s.Password);

                return new PostgreSQL.Entities.StaffEntity
                {
                    StaffCode = s.StaffCode,
                    LoginId = s.LoginId,
                    Name = s.Name,
                    PasswordHash = password.Hash.ToArray(),
                    PasswordSalt = password.Salt.ToArray(),
                    Enabled = s.Enabled,
                    RoleId = (int)s.RoleId
                };
            }).ToList();

            DateTime createdAt = DateTime.Now;
            string createdBy = "ExternalConnection";

            //職員を登録する
            await _staffRepository.UpsertStaffAsync(staffEntities, createdAt, createdBy);

            return _errorObjects;
        }

        /// <summary>
        /// 職員コードとログインIDのペアを取得する
        /// </summary>
        /// <param name="staffs"></param>
        /// <returns></returns>
        private async Task<List<Model.Standard.Staff>> GetCheckedStaffs(List<Model.Standard.Staff> staffs)
        {
            var results = new List<Model.Standard.Staff>();
            var errorCodes = new List<String>();

            // ① 登録しようとするデータからログインIDと職員コードを抽出
            var loginIdAndStaffCodePairs = staffs.Select(s => (s.StaffCode, s.LoginId)).ToList();

            // ② DBからログインIDに基づいて既存データを取得
            var existingStaffs = await _staffRepository.GetStaffsByLoginIdsAsync(loginIdAndStaffCodePairs);

            // ③ 条件に合わない職員情報を抽出
            foreach (var staff in staffs)
            {
                var matched = existingStaffs.FirstOrDefault(e => e.loginId == staff.LoginId);

                // ログインIDが一致し、職員コードが異なる場合はエラー対象
                if (matched != default && matched.staffCode != staff.StaffCode)
                {
                    errorCodes.Add(staff.StaffCode);
                }
                else
                {
                    // 条件を満たすデータのみ追加
                    results.Add(staff);
                }
            }

            // エラー対象をエラーリストに追加
            if (errorCodes.Any())
            {
                AddErrorObjects(staffs, errorCodes);
            }

            return results;
        }


        /// <summary>
        /// エラーオブジェクトに情報追加する
        /// </summary>
        /// <param name="staffs"></param>
        /// <param name="errorCodes"></param>
        private void AddErrorObjects(IEnumerable<Model.Standard.Staff> staffs, List<string> errorCodes)
        {
            // 取得できないエラーを返却用エラーオブジェクトに追加
            var errorObjects = staffs
                .Where(staff => errorCodes.Contains(staff.StaffCode))
                .Select(staff => new ErrorObject
                {
                    Code = "10001",
                    Message = $"指定されたStaffCodeに設定されたログインIDは、既に他のStaffCodeで登録済みです。Code:{staff.StaffCode}",
                    InputNote = staff.InputNote
                }).ToList();

            _errorObjects.AddRange(errorObjects);
        }
    }
}