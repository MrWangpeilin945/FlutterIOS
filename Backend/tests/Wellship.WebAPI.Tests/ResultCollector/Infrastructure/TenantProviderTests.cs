using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

namespace Wellship.WebAPI.Tests.ResultCollector.Infrastructure;

public class TenantProviderTests
{
    [Fact]
    public void IPアドレスでアクセスした場合テナント名はdefaultになる()
    {
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Scheme).Returns("https");
        request.Setup(x => x.Host).Returns(new HostString("127.0.0.1", 443));
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.Request).Returns(request.Object);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        var environmentProvider = new TenantProvider(httpContextAccessor.Object);

        environmentProvider.TenantKey.Should().Be(ITenantProvider.DefaultTenant);
    }

    [Fact]
    public void Localhostでアクセスした場合テナント名はdefaultになる()
    {
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Scheme).Returns("http");
        request.Setup(x => x.Host).Returns(new HostString("localhost", 5100));
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.Request).Returns(request.Object);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        var environmentProvider = new TenantProvider(httpContextAccessor.Object);

        environmentProvider.TenantKey.Should().Be(ITenantProvider.DefaultTenant);
    }

    [Fact]
    public void テナント名なしでアクセスした場合テナント名はdefaultになる()
    {
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Scheme).Returns("https");
        request.Setup(x => x.Host).Returns(new HostString("wellship.jp", 443));
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.Request).Returns(request.Object);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        var environmentProvider = new TenantProvider(httpContextAccessor.Object);

        environmentProvider.TenantKey.Should().Be(ITenantProvider.DefaultTenant);
    }

    [Fact]
    public void テナント名をつけてアクセスした場合テナント名はサブドメイン部分になる()
    {
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Scheme).Returns("https");
        request.Setup(x => x.Host).Returns(new HostString("tenant01.wellship.jp", 443));
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.Request).Returns(request.Object);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        var environmentProvider = new TenantProvider(httpContextAccessor.Object);

        environmentProvider.TenantKey.Should().Be("tenant01");
    }

    [Fact]
    public void 完全修飾FQDNでアクセスした場合もテナント名を取得できる()
    {
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Scheme).Returns("https");
        request.Setup(x => x.Host).Returns(new HostString("tenant01.wellship.jp.", 443));
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.Request).Returns(request.Object);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        var environmentProvider = new TenantProvider(httpContextAccessor.Object);

        environmentProvider.TenantKey.Should().Be("tenant01");
    }

    [Fact]
    public void ドメイン名が3節以上の場合は対応できない()
    {
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Scheme).Returns("https");
        request.Setup(x => x.Host).Returns(new HostString("tenant01.wellship.lg.jp", 443));
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(x => x.Request).Returns(request.Object);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        var environmentProvider = new TenantProvider(httpContextAccessor.Object);

        environmentProvider.TenantKey.Should().Be("tenant01.wellship");
    }
}