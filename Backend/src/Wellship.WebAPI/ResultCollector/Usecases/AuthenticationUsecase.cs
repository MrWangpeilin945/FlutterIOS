using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 認証ユースケース
/// </summary>
public class AuthenticationUsecase : IAuthenticationUsecase
{
    private readonly IAuthService _authService;
    private readonly IStaffRepository _staffRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AuthenticationUsecase(IAuthService authService, IStaffRepository staffRepository)
    {
        _authService = authService;
        _staffRepository = staffRepository;
    }

    /// <summary>
    /// ログインする
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="password"></param>
    public async ValueTask<string> LoginStaffAsync(string identifier, string password)
    {
        var staff = await _staffRepository.GetStaffByLoginIdAsync(identifier);
        return staff.Enabled && staff.VerifyPassword(password) ? _authService.GenerateAccessToken(staff) : throw new WellshipAuthenticationException();
    }
}
