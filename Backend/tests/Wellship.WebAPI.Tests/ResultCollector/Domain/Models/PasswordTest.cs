using FluentAssertions;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class PasswordTests
{
    [Fact]
    public void パスワードからハッシュとソルトを生成できる()
    {
        // Arrange
        var password = "testpassword";

        // Act
        var result = Password.Create(password);

        // Assert
        result.Hash.Should().NotBeEmpty();
        result.Salt.Should().NotBeEmpty();
    }

    [Fact]
    public void 正しいパスワードで検証に成功する()
    {
        // Arrange
        var password = "testpassword";
        var passwordObj = Password.Create(password);

        // Act
        var result = passwordObj.Verify(password);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void 間違ったパスワードで検証に失敗する()
    {
        // Arrange
        var password = "testpassword";
        var wrongPassword = "wrongpassword";
        var passwordObj = Password.Create(password);

        // Act
        var result = passwordObj.Verify(wrongPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void 空のパスワードで検証に失敗する()
    {
        // Arrange
        var password = "testpassword";
        var emptyPassword = "";
        var passwordObj = Password.Create(password);

        // Act
        var result = passwordObj.Verify(emptyPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ハッシュとソルトの長さを確認する()
    {
        // Arrange
        var password = "testpassword";
        var passwordObj = Password.Create(password);

        // Act
        var hashLength = passwordObj.Hash.Length;
        var saltLength = passwordObj.Salt.Length;

        // Assert
        hashLength.Should().Be(64);
        saltLength.Should().Be(128);
    }
}
