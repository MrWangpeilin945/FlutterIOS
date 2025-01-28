using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth.Settings;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

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
        var loginHasSucceeded = staff.Enabled && staff.Password.Verify(password);
        if (!loginHasSucceeded)
        {
            await _staffLoginHistoryRepository.WriteLoginFailedLogAsync(staff);
            throw new WellshipAuthenticationException();
        }
        // 新規ログイン時、セッションを識別するIDを新規発番します
        var sid = Guid.NewGuid();
        var accessToken = _authService.GenerateAccessToken(staff, sid);
        var refreshToken = RefreshToken.Create(_timeProvider.GetUtcNow().Add(_authSettings.RefreshTokenLifeTime));
        using var tran = TransactionScopeHelper.GetTransactionScope();
        await _staffLoginHistoryRepository.WriteLoginSucceededLogAsync(staff);
        await _refreshTokenRepository.UpdateRefreshTokenAsync(staff.StaffId, sid, refreshToken);
        // 新規ログイン時に対象者の期限切れトークンを削除します
        await _refreshTokenRepository.DeleteOutdatedRefreshTokensAsync(staff.StaffId);
        tran.Complete();
        return (accessToken, refreshToken.Token);
    }

    ///<inheritdoc/>
    public async ValueTask<(string accessToken, string refreshToken)> RefreshAccessTokenAsync(string accessToken, string refreshToken)
    {
        var (staff, sid, newAccessToken) = await _authService.RefreshAccessTokenAsync(accessToken, refreshToken);
        var newRefreshToken = RefreshToken.Create(_timeProvider.GetUtcNow().Add(_authSettings.RefreshTokenLifeTime));
        await _refreshTokenRepository.UpdateRefreshTokenAsync(staff.StaffId, sid, newRefreshToken);
        return (newAccessToken, newRefreshToken.Token);
    }
}
