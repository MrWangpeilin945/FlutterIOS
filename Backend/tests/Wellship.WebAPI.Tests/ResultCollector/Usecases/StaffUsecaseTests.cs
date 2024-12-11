using FluentAssertions;

using Moq;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class StaffUsecaseTests
{
    [Fact]
    public async Task 職員を取得してAPIレスポンス型に変換できる()
    {
        // Arrange
        var staffId = 123;
        var staffRepositoryMock = new Mock<IStaffRepository>();

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
        var staffUsecase = new StaffUsecase(staffRepositoryMock.Object);

        // ユースケースで変換後に期待するもの
        var expectedStaff = new APIModels.Responses.Staff()
        {
            StaffId = Guid.Parse("a5d78b8f-7e9b-4f8c-b506-8fc86b1d7681"),
            StaffName = "職員　太郎"
        };

        // Act
        var result = await staffUsecase.GetStaffAsync(staffId);

        // Assert
        result.Should().BeEquivalentTo(expectedStaff);
    }
}
