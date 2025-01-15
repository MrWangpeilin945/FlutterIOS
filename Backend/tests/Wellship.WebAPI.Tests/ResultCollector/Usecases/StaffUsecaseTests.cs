using FluentAssertions;

using Moq;

using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class StaffUsecaseTests
{
    [Fact]
    public async Task 職員を取得してAPIレスポンス型に変換できる()
    {
        // Arrange
        var staffId = Guid.Parse("a5d78b8f-7e9b-4f8c-b506-8fc86b1d7681");
        var staffRepositoryMock = new Mock<IStaffRepository>();
        var providerMock = new Mock<IStaffIdentityProvider>();

        // テーブルから取得するもの
        var staffEntity = new StaffEntity()
        {
            StaffId = Guid.Parse("a5d78b8f-7e9b-4f8c-b506-8fc86b1d7681"),
            StaffCode = "STF123",
            LoginId = "STF123",
            Name = "職員　太郎",
            Enabled = true,
            RoleId = 999
        };
        var staff = new Staff(staffEntity);
        staffRepositoryMock.Setup(r => r.GetStaffByStaffIdAsync(staffId)).ReturnsAsync(staff);
        providerMock.Setup(p => p.StaffId).Returns(Guid.Parse("a5d78b8f-7e9b-4f8c-b506-8fc86b1d7681"));

        var staffUsecase = new StaffUsecase(staffRepositoryMock.Object, providerMock.Object);

        // ユースケースで変換後に期待するもの
        var expectedStaff = new APIModels.Responses.Staff()
        {
            StaffId = Guid.Parse("a5d78b8f-7e9b-4f8c-b506-8fc86b1d7681"),
            StaffName = "職員　太郎"
        };

        // Act
        var result = await staffUsecase.GetStaffAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedStaff);
    }

    [Fact]
    public async Task 職員情報プロバイダから情報が取得できない()
    {
        // Arrange
        var staffId = Guid.Parse("a5d78b8f-7e9b-4f8c-b506-8fc86b1d7681");
        var staffRepositoryMock = new Mock<IStaffRepository>();
        var providerMock = new Mock<IStaffIdentityProvider>();

        // テーブルから取得するもの
        var staffEntity = new StaffEntity()
        {
            StaffId = Guid.Parse("a5d78b8f-7e9b-4f8c-b506-8fc86b1d7681"),
            StaffCode = "STF123",
            LoginId = "STF123",
            Name = "職員　太郎",
            Enabled = true,
            RoleId = 999
        };
        var staff = new Staff(staffEntity);
        staffRepositoryMock.Setup(r => r.GetStaffByStaffIdAsync(staffId)).ReturnsAsync(staff);
        providerMock.Setup(p => p.StaffId).Returns((Guid?)null);

        var staffUsecase = new StaffUsecase(staffRepositoryMock.Object, providerMock.Object);

        // Act & Assert
        await staffUsecase.Invoking(x => x.GetStaffAsync())
                          .Should().ThrowAsync<WellshipAuthenticationException>()
                          .WithMessage("認証情報が検証できませんでした。");
    }
}
