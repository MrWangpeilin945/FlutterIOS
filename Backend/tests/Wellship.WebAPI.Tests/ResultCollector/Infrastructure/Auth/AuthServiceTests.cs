using Moq;
using Microsoft.AspNetCore.Http;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth.Settings;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;
using Ryobi.Wellship.Core.Enums;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;

namespace Wellship.WebAPI.Tests.ResultCollector.Infrastructure.Auth;

public class AuthServiceTests
{
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public AuthServiceTests()
    {
        _tokenHandler = new JwtSecurityTokenHandler()
        {
            MapInboundClaims = false
        };
    }

    public static IEnumerable<object[]> GetStaffs()
    {
        yield return [new StaffEntity
        {
            StaffId = Guid.Parse("affd0000-0000-0000-0000-000000000001"),
            StaffCode = "AS001",
            LoginId = "S001",
            Name = "職員A",
            Enabled = true,
            RoleId = (int)Role.User,
            PasswordHash = Convert.FromBase64String("TX+Y1tzM7x6bfFxQob8oVpkSfY+avT+MJpGRzzJ54yilcsTx1T987plGIUW7ORJhfcPAPqreEzyyjzq2ufsw+w=="),
            PasswordSalt = Convert.FromBase64String("NaGPpMzTwdKiSWC8zWjj84nKm8WlXvlZSwIJ4o7kjo9/j4Hllkk61/8Vz14JVAx/KGt3GMGNE0Z/LmEb7Qfe4fAjf+aVMNkyuAywRBzwT7hUbzivt3NHohOVgIg3tYnVdLn+M4sORWIiYmq5kot9zWc02rRFeSraF4jORxQXucQ="),
        }];
        yield return [new StaffEntity
        {
            StaffId = Guid.Parse("affd0000-0000-0000-0000-000000000002"),
            StaffCode = "AS002",
            LoginId = "S002",
            Name = "管理者B",
            Enabled = true,
            RoleId = (int)Role.Admin,
            PasswordHash = Convert.FromBase64String("TX+Y1tzM7x6bfFxQob8oVpkSfY+avT+MJpGRzzJ54yilcsTx1T987plGIUW7ORJhfcPAPqreEzyyjzq2ufsw+w=="),
            PasswordSalt = Convert.FromBase64String("NaGPpMzTwdKiSWC8zWjj84nKm8WlXvlZSwIJ4o7kjo9/j4Hllkk61/8Vz14JVAx/KGt3GMGNE0Z/LmEb7Qfe4fAjf+aVMNkyuAywRBzwT7hUbzivt3NHohOVgIg3tYnVdLn+M4sORWIiYmq5kot9zWc02rRFeSraF4jORxQXucQ="),
        }];
    }

    [Theory,
    MemberData(nameof(GetStaffs))]
    public void アクセストークンのClaimが想定通りであることを確認します(StaffEntity staffEntity)
    {
        // Arrange
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Scheme).Returns("https");
        request.Setup(x => x.Host).Returns(new HostString("test.wellship.jp", 443));
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.Request).Returns(request.Object);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);
        var refreshTokenRepository = new Mock<IRefreshTokenRepository>(); ;
        var staffRepository = new Mock<IStaffRepository>(); ;

        var authSetgings = new AuthSettings()
        {
            AccessTokenLifetime = TimeSpan.FromMinutes(5),
            RefreshTokenLifeTime = TimeSpan.FromHours(9),
            SecretKey = "this is secret key of test 1234567890qwertyuiopasdfghjkl"
        };

        var staff = new Staff(staffEntity);
        var sid = Guid.Parse("12345678-1234-5678-abcd-123456789abc");
        var authService = new AuthService(authSetgings,
                                          httpContextAccessor.Object,
                                          TimeProvider.System,
                                          staffRepository.Object,
                                          refreshTokenRepository.Object);

        // Act
        var token = authService.GenerateAccessToken(staff, sid);
        var jwtSecurityToken = _tokenHandler.ReadJwtToken(token);
        var subClaimValue = jwtSecurityToken.Payload[JwtRegisteredClaimNames.Sub].ToString();
        var uniqueNameClaimValue = jwtSecurityToken.Payload[JwtRegisteredClaimNames.UniqueName].ToString();
        var roleClaimValue = jwtSecurityToken.Payload[CustomClaimTypes.Role].ToString();
        var sidClaimValue = jwtSecurityToken.Payload[JwtRegisteredClaimNames.Sid].ToString();

        // Assert
        Guid.Parse(subClaimValue!).Should().Be(staffEntity.StaffId);
        uniqueNameClaimValue.Should().Be(staffEntity.StaffCode);
        ((int)Enum.Parse<Role>(roleClaimValue!)).Should().Be(staffEntity.RoleId);
        Guid.Parse(sidClaimValue!).Should().Be(sid);
    }
}
