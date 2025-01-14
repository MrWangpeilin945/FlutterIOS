using FluentAssertions;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Moq;

using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.Core.Enums;

namespace Wellship.WebAPI.Tests.ResultCollector.Infrastructure.Auth;

public class StaffIdentityFromHttpContextProviderTests
{
    [Fact]
    public void HttpContextから正しい認証情報を取得しプロパティから返す()
    {
        // arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new(JwtRegisteredClaimNames.Sub, "d7ab05ac-b97d-4ab0-b719-2635e67e3bf8"),
                new(JwtRegisteredClaimNames.UniqueName, "S001"),
                new(CustomClaimTypes.Role, "Admin")
            ]
        ));

        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.User).Returns(claimsPrincipal);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        // act
        var staffIdentity = new StaffIdentityFromHttpContextProvider(httpContextAccessor.Object);

        // Assert
        staffIdentity.StaffId.Should().Be("d7ab05ac-b97d-4ab0-b719-2635e67e3bf8");
        staffIdentity.StaffCode.Should().Be("S001");
        staffIdentity.Role.Should().Be(Role.Admin);
    }

    [Fact]
    public void HttpContextから認証情報を取得できなかった場合プロパティがnullを返す()
    {
        // arrange
        var claimsPrincipal = new ClaimsPrincipal();
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.User).Returns(claimsPrincipal);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        // act
        var staffIdentity = new StaffIdentityFromHttpContextProvider(httpContextAccessor.Object);

        // Assert
        staffIdentity.StaffId.Should().BeNull();
        staffIdentity.StaffCode.Should().BeNull();
        staffIdentity.Role.Should().BeNull();
    }

    [Fact]
    public void HttpContextから不正な認証情報を取得した場合プロパティがnullを返す()
    {
        // arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new(JwtRegisteredClaimNames.Sub, "bad-guid"),
                new(CustomClaimTypes.Role, "bad-role")
            ]
        ));

        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.User).Returns(claimsPrincipal);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        // act
        var staffIdentity = new StaffIdentityFromHttpContextProvider(httpContextAccessor.Object);

        // Assert
        staffIdentity.StaffId.Should().BeNull();
        staffIdentity.Role.Should().BeNull();
    }
}