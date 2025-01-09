using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth.Settings;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 認証ユースケース
/// </summary>
/// <remarks>
/// コンストラクタ
/// </remarks>
public class AuthenticationUsecase(IAuthService authService,
                                   AuthSettings authSettings,
                                   IStaffRepository staffRepository,
                                   IRefreshTokenRepository refreshTokenRepository,
                                   IStaffLoginHistoryRepository staffLoginHistoryRepository,
                                   TimeProvider timeProvider) : IAuthenticationUsecase
{
    private readonly IAuthService _authService = authService;
    private readonly AuthSettings _authSettings = authSettings;
    private readonly IStaffRepository _staffRepository = staffRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IStaffLoginHistoryRepository _staffLoginHistoryRepository = staffLoginHistoryRepository;
    private readonly TimeProvider _timeProvider = timeProvider;

    ///<inheritdoc/>
    public async ValueTask<(string accessToken, string refreshToken)> LoginStaffAsync(string identifier, string password)
    {
        var staff = await _staffRepository.GetStaffByLoginIdAsync(identifier);
        var loginHasSucceeded = staff.Enabled && staff.VerifyPassword(password);
        if (!loginHasSucceeded)
        {
            await _staffLoginHistoryRepository.WriteLoginFailedLogAsync(staff);
            throw new WellshipAuthenticationException();
        }
        var accessToken = _authService.GenerateAccessToken(staff);
        var refreshToken = RefreshToken.Create(_timeProvider.GetUtcNow().Add(_authSettings.RefreshTokenLifeTime));
        await _staffLoginHistoryRepository.WriteLoginSucceededLogAsync(staff);
        await _refreshTokenRepository.ExpireRefreshTokenAsync(staff.StaffId);
        await _refreshTokenRepository.UpdateRefreshTokenAsync(staff.StaffId, refreshToken);
        return (accessToken, refreshToken.Token);
    }

    ///<inheritdoc/>
    public async ValueTask<(string accessToken, string refreshToken)> RefreshAccessTokenAsync(string accessToken, string refreshToken)
    {
        var (staff, newAccessToken) = await _authService.RefreshAccessTokenAsync(accessToken, refreshToken);
        var newRefreshToken = RefreshToken.Create(_timeProvider.GetUtcNow().Add(_authSettings.RefreshTokenLifeTime));
        await _refreshTokenRepository.UpdateRefreshTokenAsync(staff.StaffId, newRefreshToken);
        return (newAccessToken, newRefreshToken.Token);
    }
}
