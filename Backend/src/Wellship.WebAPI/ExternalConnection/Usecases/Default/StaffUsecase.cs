using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2011_職員を登録する
/// </summary>
public class StaffUsecase : IStaffUsecase
{
    private readonly IStaffRepository _staffRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// ユースケースを作成する
    /// </summary>
    /// <param name="staffRepository"></param>
    /// <param name="timeProvider"></param>
    public StaffUsecase(IStaffRepository staffRepository, TimeProvider timeProvider)
    {
        _staffRepository = staffRepository;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 職員を登録する
    /// </summary>
    /// <param name="staffs"></param>
    /// <returns></returns>
    public async Task<List<ErrorObject>> StoreStaffsAsync(List<Staff> staffs)
    {
        List<ErrorObject> errorObjects = new List<ErrorObject>();

        // ログインIDに紐づく職員情報を取得する
        var loginIdAndStaffCodePairs = staffs.Select(s => (s.StaffCode, s.LoginId)).ToList();
        var existingStaffs = await _staffRepository.GetStaffsByLoginIdsAsync(loginIdAndStaffCodePairs);

        // WARNING検証
        var warningStaffs = new List<Staff>();

        // 必須項目の空値のチェック
        var spaceCheckProperties = new[]
        {
            "StaffCode",    // 職員コード
            "LoginId",      // ログインID
            "Name",         // 職員名
            "Password"      // パスワード
        };
        // チェックするプロパティ一覧をメソッドに渡して空値チェックする
        foreach (var warning in ValidationChecker.SpaceCheckProperties(staffs, spaceCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをStaffにキャストしてワーニングリストに追加する
            if (warning is Staff staff)
            {
                warningStaffs.Add(staff);
            }
        }

        // 文字数のチェック
        var stringLengthCheckProperties = new[]
        {
            "LoginId",      // ログインID　20桁以下
            "Password"      // パスワード  20桁以下
        };
        var stringLengths = new[] { 20, 20 };
        var compars = new[] { -1, -1 };
        // チェックするプロパティ一覧をメソッドに渡して文字数チェックを行う
        foreach (var warning in ValidationChecker.StringLengthCheckProperties(staffs, stringLengthCheckProperties, stringLengths, compars, errorObjects))
        {
            // エラーのオブジェクトをStaffにキャストしてワーニングリストに追加する
            if (warning is Staff staff)
            {
                warningStaffs.Add(staff);
            }
        }

        // 文字形式のチェック
        var stringPatternCheckProperties = new[]
        {
            "LoginId"       // ログインID
        };
        var patterns = new[] { @"^[a-zA-Z0-9]+$" };
        // チェックするプロパティ一覧をメソッドに渡して文字形式チェックを行う
        foreach (var warning in ValidationChecker.StringPatternCheckProperties(staffs, stringPatternCheckProperties, patterns, errorObjects))
        {
            // エラーのオブジェクトをStaffにキャストしてワーニングリストに追加する
            if (warning is Staff staff)
            {
                warningStaffs.Add(staff);
            }
        }

        // キー重複チェック
        var duplicateCheckProperties = new[]
        {
            "StaffCode",    // 職員コード
            "LoginId"       // ログインID
        };
        // チェックするプロパティ一覧をメソッドに渡して重複チェックを行う
        foreach (var warning in ValidationChecker.DuplicateCheckProperties(staffs, duplicateCheckProperties, errorObjects))
        {
            // エラーのオブジェクトをStaffにキャストしてワーニングリストに追加する
            if (warning is Staff staff)
            {
                warningStaffs.Add(staff);
            }
        }

        // ログインIDが異なる職員コードのチェック
        /*
        foreach (var warning in staffs.Where(x => existingStaffs.Any(staff => staff.LoginId == x.LoginId && staff.StaffCode != x.StaffCode)))
        {
            // エラーのオブジェクトをStaffにキャストしてワーニングリストに追加する
            if (warning is Staff staff)
            {
                warningStaffs.Add(staff);
                errorObjects.Add(new ErrorObject
                {
                    Code = "10002",
                    Message = $"指定されたLoginIdが既に登録済みです。Code:{staff.LoginId}",
                    InputNote = staff.InputNote
                });
            }
        }
        */
        foreach (var staff in staffs)
        {
            var matched = existingStaffs.Where(e => e.LoginId == staff.LoginId)
                                        .FirstOrDefault();

            // ログインIDが一致し、職員コードが異なる場合はエラー対象
            if (matched != default && matched.StaffCode != staff.StaffCode)
            {
                warningStaffs.Add(staff);
                errorObjects.Add(new ErrorObject
                {
                    Code = "10002",
                    Message = $"指定されたLoginIdが既に登録済みです。Code:{staff.LoginId}",
                    InputNote = staff.InputNote
                });
            }
        }

        var staffEntities = staffs.Except(warningStaffs)
                                  .Select(s =>
            {
                // パスワードからハッシュとソルトを取得
                ResultCollector.Domain.Models.Password password = ResultCollector.Domain.Models.Password.Create(s.Password);
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

        //職員を登録する
        await _staffRepository.UpsertStaffAsync(staffEntities, _timeProvider.GetUtcNow(), "ExternalConnection");

        return errorObjects;
    }
}