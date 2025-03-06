using FluentAssertions;

using Moq;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth.Settings;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class AuthenticationUsecaseTests
{
    private readonly Mock<IAuthService> _authService;
    private readonly Mock<IStaffRepository> _staffRepository;
    private readonly AuthSettings _authSettings;
    private readonly Mock<IStaffLoginHistoryRepository> _staffLoginHistoryRepository;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository;
    private readonly TimeProvider _timeProvider;

    public AuthenticationUsecaseTests()
    {
        _authService = new Mock<IAuthService>();
        _staffRepository = new Mock<IStaffRepository>();
        _authSettings = new AuthSettings()
        {
            AccessTokenLifetime = TimeSpan.FromMinutes(5),
            RefreshTokenLifeTime = TimeSpan.FromHours(12),
            SecretKey = "jwt-signing-secret-key",
        };
        _staffLoginHistoryRepository = new Mock<IStaffLoginHistoryRepository>();
        _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task パスワードの検証に成功した場合アクセストークンとリフレッシュトークンが返ってくる()
    {
        // Arrange
        var hash = "TX+Y1tzM7x6bfFxQob8oVpkSfY+avT+MJpGRzzJ54yilcsTx1T987plGIUW7ORJhfcPAPqreEzyyjzq2ufsw+w==";
        var salt = "NaGPpMzTwdKiSWC8zWjj84nKm8WlXvlZSwIJ4o7kjo9/j4Hllkk61/8Vz14JVAx/KGt3GMGNE0Z/LmEb7Qfe4fAjf+aVMNkyuAywRBzwT7hUbzivt3NHohOVgIg3tYnVdLn+M4sORWIiYmq5kot9zWc02rRFeSraF4jORxQXucQ=";

        var staffEntity = new StaffEntity()
        {
            StaffId = Guid.Parse("affd0000-0000-0000-0000-000000000001"),
            StaffCode = "AS001",
            LoginId = "S001",
            Name = "職員A",
            Enabled = true,
            RoleId = (int)Role.User,
            PasswordHash = Convert.FromBase64String(hash),
            PasswordSalt = Convert.FromBase64String(salt),
        };
        var staff = new Staff(staffEntity);
        _staffRepository.Setup(x => x.GetStaffByLoginIdAsync(It.IsAny<string>())).ReturnsAsync(staff);
        _authService.Setup(x => x.GenerateAccessToken(It.IsAny<Staff>(), It.IsAny<Guid>())).Returns("accessToken");
        _staffLoginHistoryRepository.Setup(x => x.WriteLoginSucceededLogAsync(It.IsAny<Staff>())).Returns(ValueTask.CompletedTask);
        _refreshTokenRepository.Setup(x => x.ExpireRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(ValueTask.CompletedTask);
        _refreshTokenRepository.Setup(x => x.UpdateRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<RefreshToken>())).Returns(ValueTask.CompletedTask);

        var usecase = new AuthenticationUsecase(_authService.Object, _authSettings, _staffRepository.Object,
                                                _refreshTokenRepository.Object, _staffLoginHistoryRepository.Object, _timeProvider);

        // Act
        var (accessToken, refreshToken) = await usecase.LoginStaffAsync("S001", "syokuinA");

        // Assert
        accessToken.Should().NotBeNullOrWhiteSpace();
        refreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task 誤ったパスワードを検証した場合認証例外が発生する()
    {
        // Arrange
        var hash = "TX+Y1tzM7x6bfFxQob8oVpkSfY+avT+MJpGRzzJ54yilcsTx1T987plGIUW7ORJhfcPAPqreEzyyjzq2ufsw+w==";
        var salt = "NaGPpMzTwdKiSWC8zWjj84nKm8WlXvlZSwIJ4o7kjo9/j4Hllkk61/8Vz14JVAx/KGt3GMGNE0Z/LmEb7Qfe4fAjf+aVMNkyuAywRBzwT7hUbzivt3NHohOVgIg3tYnVdLn+M4sORWIiYmq5kot9zWc02rRFeSraF4jORxQXucQ=";

        var staffEntity = new StaffEntity()
        {
            StaffId = Guid.Parse("affd0000-0000-0000-0000-000000000001"),
            StaffCode = "AS001",
            LoginId = "S001",
            Name = "職員A",
            Enabled = true,
            RoleId = (int)Role.User,
            PasswordHash = Convert.FromBase64String(hash),
            PasswordSalt = Convert.FromBase64String(salt),
        };
        var staff = new Staff(staffEntity);
        _staffRepository.Setup(x => x.GetStaffByLoginIdAsync(It.IsAny<string>())).ReturnsAsync(staff);
        _authService.Setup(x => x.GenerateAccessToken(It.IsAny<Staff>(), It.IsAny<Guid>())).Returns("accessToken");
        _staffLoginHistoryRepository.Setup(x => x.WriteLoginFailedLogAsync(It.IsAny<Staff>())).Returns(ValueTask.CompletedTask);

        var usecase = new AuthenticationUsecase(_authService.Object, _authSettings, _staffRepository.Object,
                                                _refreshTokenRepository.Object, _staffLoginHistoryRepository.Object, _timeProvider);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<WellshipAuthenticationException>(async () => await usecase.LoginStaffAsync("S001", "BadPassword"));
    }

    [Fact]
    public async Task リフレッシュトークンの検証に成功した場合新しいアクセストークンとリフレッシュトークンが返ってくる()
    {
        // Arrange
        var oldAccessToken = "oldAccessToken";
        var oldRefreshToken = "oldRefreshToken";
        var sid = Guid.Parse("abcd0000-0000-0000-0000-000000000001");
        var newAccessToken = "newAccessToken";
        var newRefreshToken = RefreshToken.Create(_timeProvider.GetUtcNow().Add(_authSettings.RefreshTokenLifeTime));

        var hash = "TX+Y1tzM7x6bfFxQob8oVpkSfY+avT+MJpGRzzJ54yilcsTx1T987plGIUW7ORJhfcPAPqreEzyyjzq2ufsw+w==";
        var salt = "NaGPpMzTwdKiSWC8zWjj84nKm8WlXvlZSwIJ4o7kjo9/j4Hllkk61/8Vz14JVAx/KGt3GMGNE0Z/LmEb7Qfe4fAjf+aVMNkyuAywRBzwT7hUbzivt3NHohOVgIg3tYnVdLn+M4sORWIiYmq5kot9zWc02rRFeSraF4jORxQXucQ=";

        var staffEntity = new StaffEntity()
        {
            StaffId = Guid.Parse("affd0000-0000-0000-0000-000000000001"),
            StaffCode = "AS001",
            LoginId = "S001",
            Name = "職員A",
            Enabled = true,
            RoleId = (int)Role.User,
            PasswordHash = Convert.FromBase64String(hash),
            PasswordSalt = Convert.FromBase64String(salt),
        };

        var staff = new Staff(staffEntity);

        _authService.Setup(x => x.RefreshAccessTokenAsync(oldAccessToken, oldRefreshToken)).ReturnsAsync((staff, sid, newAccessToken));
        _refreshTokenRepository.Setup(x => x.UpdateRefreshTokenAsync(staff.StaffId, sid, It.IsAny<RefreshToken>())).Returns(ValueTask.CompletedTask);

        var usecase = new AuthenticationUsecase(_authService.Object, _authSettings, _staffRepository.Object,
                                                _refreshTokenRepository.Object, _staffLoginHistoryRepository.Object, _timeProvider);

        // Act
        var (returnedAccessToken, returnedRefreshToken) = await usecase.RefreshAccessTokenAsync(oldAccessToken, oldRefreshToken);

        // Assert
        returnedAccessToken.Should().Be(newAccessToken);
        returnedRefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task 職員が存在しない場合認証例外が発生する()
    {
        // Arrange
        _staffRepository.Setup(x => x.GetStaffByLoginIdAsync(It.IsAny<string>())).ThrowsAsync(new StaffNotFoundException());

        var usecase = new AuthenticationUsecase(_authService.Object, _authSettings, _staffRepository.Object,
                                                _refreshTokenRepository.Object, _staffLoginHistoryRepository.Object, _timeProvider);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<WellshipAuthenticationException>(async () => await usecase.LoginStaffAsync("S001", "password"));
    }
}
