using FluentAssertions;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;

public class HomeMenuSettingsTest
{
    [Fact]
    public void 検査メニュー選択画面_一般()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.検査メニュー選択画面;
        var role = Role.User;
        var expectedConditions = new[] { "PlaceScheduleSelected", "PlaceScheduleUnlocked" };

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeTrue();
    }

    [Fact]
    public void 検査メニュー選択画面_管理者()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.検査メニュー選択画面;
        var role = Role.Admin;
        var expectedConditions = new[] { "PlaceScheduleSelected", "PlaceScheduleUnlocked" };

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeTrue();
    }

    [Fact]
    public void 進捗画面_一般()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.進捗画面;
        var role = Role.User;
        var expectedConditions = new[] { "PlaceScheduleSelected" };

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeTrue();
    }

    [Fact]
    public void 進捗画面_管理者()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.進捗画面;
        var role = Role.Admin;
        var expectedConditions = new[] { "PlaceScheduleSelected" };

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeTrue();
    }

    [Fact]
    public void 会場ロック画面_一般()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.会場ロック画面;
        var role = Role.User;
        var expectedConditions = new[] { "PlaceScheduleSelected", "PlaceScheduleUnlocked" };

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeFalse();
    }

    [Fact]
    public void 会場ロック画面_管理者()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.会場ロック画面;
        var role = Role.Admin;
        var expectedConditions = new[] { "PlaceScheduleSelected", "PlaceScheduleUnlocked" };

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeTrue();
    }

    [Fact]
    public void 検査結果出力画面_一般()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.検査結果出力画面;
        var role = Role.User;
        var expectedConditions = Array.Empty<string>();

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeFalse();
    }

    [Fact]
    public void 検査結果出力画面_管理者()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.検査結果出力画面;
        var role = Role.Admin;
        var expectedConditions = Array.Empty<string>();

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeTrue();
    }

    [Fact]
    public void 検査結果出力履歴画面_一般()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.検査結果出力履歴画面;
        var role = Role.User;
        var expectedConditions = Array.Empty<string>();

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeFalse();
    }

    [Fact]
    public void 検査結果出力履歴画面_管理者()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = ScreenPathConsts.検査結果出力履歴画面;
        var role = Role.Admin;
        var expectedConditions = Array.Empty<string>();

        // Act
        var availableConditions = settings.GetAvailableConditions(path);
        var canRoleUseFeature = settings.CanRoleUseFeature(path, role);

        // Assert
        availableConditions.Should().BeEquivalentTo(expectedConditions);
        canRoleUseFeature.Should().BeTrue();
    }

    [Fact]
    public void 使用可能条件取得_マスタに記載したパスが設定に存在しない()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = "dummy";

        // Act
        Action act = () => settings.GetAvailableConditions(path);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("設定された画面パスが無効です。*");
    }

    [Fact]
    public void ロール条件判断_マスタに記載したパスが設定に存在しない()
    {
        // Arrange
        var settings = new HomeMenuSettings();
        var path = "dummy";
        var role = Role.Admin;

        // Act
        Action act = () => settings.CanRoleUseFeature(path, role);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("設定された画面パスが無効です。*");
    }

}
