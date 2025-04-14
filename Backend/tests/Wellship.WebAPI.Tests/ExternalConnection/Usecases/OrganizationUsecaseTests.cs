using Moq;
using FluentAssertions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class OrganizationUsecaseTests
{
    private readonly Mock<IOrganizationRepository> _organizationRepositoryMock;
    private readonly TimeProvider _timeProvider;

    public OrganizationUsecaseTests()
    {
        _organizationRepositoryMock = new Mock<IOrganizationRepository>();
        _timeProvider = TimeProvider.System;
    }
    [Fact]
    public async Task 空値のチェック_団体コードで1件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Code", InputNote  = "UT2008"}
        };
        var usecase = new OrganizationUsecase(_organizationRepositoryMock.Object, _timeProvider);
        var request = new List<Organization>
        {
            new Organization { Code = "", Name = "両備健康組合", InputNote = "UT2008" }
        };
        // Act
        var errors = await usecase.StoreOrganizationsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_団体名で1件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Name", InputNote  = "UT2008"}
        };
        var usecase = new OrganizationUsecase(_organizationRepositoryMock.Object, _timeProvider);
        var request = new List<Organization>
        {
            new Organization { Code = "8931", Name = "", InputNote = "UT2008" }
        };
        // Act
        var errors = await usecase.StoreOrganizationsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 重複キーのチェック_団体コードで2件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。Code:5963", InputNote  = "UT2008"},
            new(){Code = "10003", Message = "キー項目が重複しています。Code:5963", InputNote  = "UT2008"}
        };
        var usecase = new OrganizationUsecase(_organizationRepositoryMock.Object, _timeProvider);
        var request = new List<Organization>
        {
            new Organization { Code = "5963", Name = "両備工業", InputNote = "UT2008" },
            new Organization { Code = "5963", Name = "両備商工", InputNote = "UT2008" }
        };
        // Act
        var errors = await usecase.StoreOrganizationsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

}
