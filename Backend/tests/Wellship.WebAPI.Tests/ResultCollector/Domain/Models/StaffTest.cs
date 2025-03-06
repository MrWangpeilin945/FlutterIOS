using FluentAssertions;

using Microsoft.Extensions.Logging;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models
{
    public class StaffTest
    {
        [Fact]
        public void 職員オブジェクトが生成できる()
        {
            // Arrange

            // テーブルから取得するもの
            var staffEntity = new StaffEntity()
            {
                StaffId = Guid.Parse("a5d78b8f-7e9b-4f8c-b506-8fc86b1d7681"),
                StaffCode = "STF123",
                LoginId = "123",
                Name = "一般職員　太郎",
                Enabled = true,
                RoleId = 10,
                PasswordHash = new byte[64],
                PasswordSalt = new byte[128]
            };

            // Act
            var staff = new Staff(staffEntity);

            // Assert
            staff.StaffId.Should().Be(Guid.Parse("a5d78b8f-7e9b-4f8c-b506-8fc86b1d7681"));
            staff.StaffCode.Should().Be("STF123");
            staff.LoginId.Should().Be("123");
            staff.Name.Should().Be("一般職員　太郎");
            staff.Enabled.Should().BeTrue();
            staff.Role.Should().Be(Core.Enums.Role.User);
            staff.Password.Hash.Should().NotBeEmpty();
            staff.Password.Salt.Should().NotBeEmpty();
        }
    }
}
