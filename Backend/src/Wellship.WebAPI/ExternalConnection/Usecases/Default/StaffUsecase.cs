using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using System.Text.RegularExpressions;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2011_職員を登録する
/// </summary>
public class StaffUsecase : IStaffUsecase
{
    private readonly List<ErrorObject> _errorObjects;
    private readonly IStaffRepository _staffRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// ユースケースを作成する
    /// </summary>
    /// <param name="dbConnectionProvider"></param>
    /// <param name="staffRepository"></param>
    /// <param name="timeProvider"></param>
    public StaffUsecase(IDbConnectionProvider dbConnectionProvider, IStaffRepository staffRepository,
                        TimeProvider timeProvider)
    {
        _errorObjects = new List<ErrorObject>();
        _staffRepository = staffRepository;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 職員を登録する
    /// </summary>
    /// <param name="staffs"></param>
    /// <returns></returns>
    public async Task<List<ErrorObject>> StoreStaffsAsync(List<Model.Standard.Staff> staffs)
    {
        _errorObjects.Clear();

        // 必須の確認
        var insertStaffsByRequireds = GetCheckedRequired(staffs);

        // 文字長の確認
        var insertStaffsByLength = GetCheckedLength(staffs);

        // 文字種類の確認
        var insertStaffsByFormat = GetCheckedFormat(staffs);

        // キー重複の確認
        var insertStaffsByLoginId = GetCheckedDuplicated(staffs);

        var commonInsertStaffs = insertStaffsByRequireds.Intersect(insertStaffsByLength)
                                                        .Intersect(insertStaffsByFormat)
                                                        .Intersect(insertStaffsByLoginId)
                                                        .ToList();

        // ログインIDの確認
        var insertStaffs = await GetCheckedStaffs(commonInsertStaffs);

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

        var createdAt = _timeProvider.GetUtcNow();
        string createdBy = "ExternalConnection";

        //職員を登録する
        await _staffRepository.UpsertStaffAsync(staffEntities, createdAt, createdBy);

        return _errorObjects;
    }

    /// <summary>
    /// 必須チェック済みのリストを取得する
    /// </summary>
    /// <param name="staffs"></param>
    /// <returns></returns>
    private List<Model.Standard.Staff> GetCheckedRequired(List<Model.Standard.Staff> staffs)
    {
        // WARNING検証
        // 未入力(StaffCode)
        var requiredStaffCodeData = staffs.Where(x => string.IsNullOrWhiteSpace(x.StaffCode));
        if (requiredStaffCodeData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredStaffCodeData, "StaffCode");
        }

        // 未入力(LoginId)
        var requiredLoginIdData = staffs.Where(x => string.IsNullOrWhiteSpace(x.LoginId));
        if (requiredLoginIdData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredLoginIdData, "LoginId");
        }

        // 未入力(Password)
        var requirePasswordData = staffs.Where(x => string.IsNullOrWhiteSpace(x.Password));
        if (requirePasswordData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requirePasswordData, "Password");
        }

        // 未入力(Name)
        var requiredNameData = staffs.Where(x => string.IsNullOrWhiteSpace(x.Name));
        if (requiredNameData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddRequiredDataErrorObjects(requiredNameData, "Name");
        }

        return staffs.Except(requiredStaffCodeData)
                     .Except(requiredLoginIdData)
                     .Except(requirePasswordData)
                     .Except(requiredNameData)
                     .ToList();
    }

    /// <summary>
    /// 文字長チェック済みのリストを取得する
    /// </summary>
    /// <param name="staffs"></param>
    /// <returns></returns>
    private List<Model.Standard.Staff> GetCheckedLength(List<Model.Standard.Staff> staffs)
    {
        // WARNING検証
        // 文字数超過(LoginId)
        var overdLoginIdData = staffs.Where(x => x.LoginId.Length > 20);
        if (overdLoginIdData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddLengthDataErrorObjects(overdLoginIdData.Select(x => (x.LoginId, x.InputNote)), "LoginId");
        }

        // 文字数超過(Password)
        var overdPasswordData = staffs.Where(x => x.Password.Length > 20);
        if (overdPasswordData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddLengthDataErrorObjects(overdPasswordData.Select(x => (x.Password, x.InputNote)), "Password");
        }

        return staffs.Except(overdLoginIdData)
                     .Except(overdPasswordData)
                     .ToList();
    }

    /// <summary>
    /// 形式チェック済みのリストを取得する
    /// </summary>
    /// <param name="staffs"></param>
    /// <returns></returns>
    private List<Model.Standard.Staff> GetCheckedFormat(List<Model.Standard.Staff> staffs)
    {
        // WARNING検証
        // 形式(LoginId)
        var formatLoginIdData = staffs.Where(x => !Regex.IsMatch(x.LoginId, @"^[a-zA-Z0-9]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500)));
        if (formatLoginIdData.Any())
        {
            // 返却用エラーオブジェクトに追加
            AddFormatDataErrorObjects(formatLoginIdData.Select(x => (x.LoginId, x.InputNote)), "LoginId");
        }

        return staffs.Except(formatLoginIdData)
                     .ToList();
    }
    /// <summary>
    /// 重複チェック済みのリストを取得する
    /// </summary>
    /// <param name="staffs"></param>
    /// <returns></returns>
    private List<Model.Standard.Staff> GetCheckedDuplicated(List<Model.Standard.Staff> staffs)
    {
        // WARNING検証
        // キー重複(StaffCode)
        var duplicatedStaffCodes = staffs.GroupBy(x => x.StaffCode).Where(x => x.Count() > 1).Select(x => x.Key).ToHashSet();
        if (duplicatedStaffCodes.Any())
        {
            var duplicatedData = staffs.Where(x => duplicatedStaffCodes.Contains(x.StaffCode)).Select(x => (x.StaffCode, x.InputNote));

            // 返却用エラーオブジェクトに追加
            AddDuplicateErrorObjects(duplicatedData, "StaffCode");
        }

        // キー重複(LoginId)
        var duplicatedLoginIdCodes = staffs.GroupBy(x => x.LoginId).Where(x => x.Count() > 1).Select(x => x.Key).ToHashSet();
        if (duplicatedLoginIdCodes.Any())
        {
            var duplicatedData = staffs.Where(x => duplicatedLoginIdCodes.Contains(x.LoginId)).Select(x => (x.LoginId, x.InputNote));

            // 返却用エラーオブジェクトに追加
            AddDuplicateErrorObjects(duplicatedData, "LoginId");
        }

        return staffs.Where(x => !duplicatedStaffCodes.Contains(x.StaffCode))
                     .Where(x => !duplicatedLoginIdCodes.Contains(x.LoginId))
                     .ToList();
    }

    /// <summary>
    /// 職員コードとログインIDのペアを取得する
    /// </summary>
    /// <param name="staffs"></param>
    /// <returns></returns>
    private async Task<List<Model.Standard.Staff>> GetCheckedStaffs(List<Model.Standard.Staff> staffs)
    {
        var results = new List<Model.Standard.Staff>();
        var errorCodes = new List<string>();

        if (staffs == null || staffs.Count() <= 0)
        {
            return results;
        }

        // ① 登録しようとするデータからログインIDと職員コードを抽出
        var loginIdAndStaffCodePairs = staffs.Select(s => (s.StaffCode, s.LoginId)).ToList();

        // ② DBからログインIDに基づいて既存データを取得
        var existingStaffs = await _staffRepository.GetStaffsByLoginIdsAsync(loginIdAndStaffCodePairs);

        // ③ 条件に合わない職員情報を抽出
        foreach (var staff in staffs)
        {
            var matched = existingStaffs.Where(e => e.LoginId == staff.LoginId)
                                        .FirstOrDefault();

            // ログインIDが一致し、職員コードが異なる場合はエラー対象
            if (matched != default && matched.StaffCode != staff.StaffCode)
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
    /// エラーオブジェクトに情報追加する(必須項目エラー）
    /// </summary>
    /// <param name="requiredData"></param>
    /// <param name="itemName"></param>
    private void AddRequiredDataErrorObjects(IEnumerable<Model.Standard.Staff> requiredData, string itemName)
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
    /// エラーオブジェクトに情報追加する(文字長エラー）
    /// </summary>
    /// <param name="errorData"></param>
    /// <param name="itemName"></param>
    private void AddLengthDataErrorObjects(IEnumerable<(string Value, string InputNote)> errorData, string itemName)
    {
        var errorObjects = errorData
            .Select(r => new ErrorObject
            {
                Code = "10005",
                Message = $"制限数を超えています。{itemName}:{r.Value}",
                InputNote = r.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(形式エラー）
    /// </summary>
    /// <param name="errorData"></param>
    /// <param name="itemName"></param>
    private void AddFormatDataErrorObjects(IEnumerable<(string Value, string InputNote)> errorData, string itemName)
    {
        var errorObjects = errorData
            .Select(r => new ErrorObject
            {
                Code = "10006",
                Message = $"値の形式が無効です。{itemName}:{r.Value}",
                InputNote = r.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
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
                Code = "10002",
                Message = $"指定されたLoginIdが既に登録済みです。Code:{staff.LoginId}",
                InputNote = staff.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }

    /// <summary>
    /// エラーオブジェクトに情報追加する(キーが重複するレコード）
    /// </summary>
    /// <param name="errorData"></param>
    private void AddDuplicateErrorObjects(IEnumerable<(string Value, string InputNote)> errorData, string itemName)
    {
        var errorObjects = errorData
            .Select(d => new ErrorObject
            {
                Code = "10003",
                Message = $"キー項目が重複しています。{itemName}:{d.Value}",
                InputNote = d.InputNote
            }).ToList();

        _errorObjects.AddRange(errorObjects);
    }
}