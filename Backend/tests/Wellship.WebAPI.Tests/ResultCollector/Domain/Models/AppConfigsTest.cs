using FluentAssertions;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class AppConfigsTest
{
    [Fact]
    public void アクセストークン有効期限_設定が存在しない場合はデフォルト値を返す()
    {
        // Arrange
        var appConfigs = new AppConfigs(new Dictionary<string, string>());

        // Act
        var result = appConfigs.AccessTokenLifetime;

        // Assert
        result.Should().Be(AppConfigConsts.AccessTokenLifetime.DefaultValue);
    }

    [Fact]
    public void アクセストークン有効期限_設定が存在する場合はその値を返す()
    {
        // Arrange
        var settings = new Dictionary<string, string> { { AppConfigConsts.AccessTokenLifetime.Key, "15" } };
        var appConfigs = new AppConfigs(settings);

        // Act
        var result = appConfigs.AccessTokenLifetime;

        // Assert
        result.Should().Be(15);
    }

    [Fact]
    public void アクセストークン有効期限_設定が無効な場合はデフォルト値を返す()
    {
        // Arrange
        var settings = new Dictionary<string, string> { { AppConfigConsts.AccessTokenLifetime.Key, "invalid" } };
        var appConfigs = new AppConfigs(settings);

        // Act
        var result = appConfigs.AccessTokenLifetime;

        // Assert
        result.Should().Be(AppConfigConsts.AccessTokenLifetime.DefaultValue);
    }

    [Fact]
    public void リフレッシュトークン有効期限_設定が存在しない場合はデフォルト値を返す()
    {
        // Arrange
        var appConfigs = new AppConfigs(new Dictionary<string, string>());

        // Act
        var result = appConfigs.RefreshTokenLifeTime;

        // Assert
        result.Should().Be(AppConfigConsts.RefreshTokenLifeTime.DefaultValue);
    }

    [Fact]
    public void リフレッシュトークン有効期限_設定が存在する場合はその値を返す()
    {
        // Arrange
        var settings = new Dictionary<string, string> { { AppConfigConsts.RefreshTokenLifeTime.Key, "1440" } };
        var appConfigs = new AppConfigs(settings);

        // Act
        var result = appConfigs.RefreshTokenLifeTime;

        // Assert
        result.Should().Be(1440);
    }

    [Fact]
    public void リフレッシュトークン有効期限_設定が無効な場合はデフォルト値を返す()
    {
        // Arrange
        var settings = new Dictionary<string, string> { { AppConfigConsts.RefreshTokenLifeTime.Key, "invalid" } };
        var appConfigs = new AppConfigs(settings);

        // Act
        var result = appConfigs.RefreshTokenLifeTime;

        // Assert
        result.Should().Be(AppConfigConsts.RefreshTokenLifeTime.DefaultValue);
    }

    [Fact]
    public void シークレットキー_設定が存在しない場合は例外をスローする()
    {
        // Arrange
        var appConfigs = new AppConfigs(new Dictionary<string, string>());

        // Act
        Action act = () => { var result = appConfigs.SecretKey; };

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("SecretKeyは必須設定です。*");
    }

    [Fact]
    public void シークレットキー_設定が存在する場合はその値を返す()
    {
        // Arrange
        var settings = new Dictionary<string, string> { { AppConfigConsts.SecretKey.Key, "my_secret_key" } };
        var appConfigs = new AppConfigs(settings);

        // Act
        var result = appConfigs.SecretKey;

        // Assert
        result.Should().Be("my_secret_key");
    }
}
