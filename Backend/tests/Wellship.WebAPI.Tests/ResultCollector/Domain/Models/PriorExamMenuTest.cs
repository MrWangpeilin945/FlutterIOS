using FluentAssertions;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;
public class PriorExamMenuTests
{
    [Fact]
    public void 前提検査メニューのうち未受診の検査メニューがある()
    {
        // Arrange

        // 前提検査メニューの設定
        var currentExamMenuId = 6;
        var priorExamMenuIdList = new List<int>() { 1, 2, 3, 5 };
        var priorExamMenu = new PriorExamMenu(currentExamMenuId, priorExamMenuIdList);

        // 未受診の検査メニュー
        var unexaminedMenuIds = new List<int>() { 2, 3, 8, 9 };
        var expected = new List<int>() { 2, 3 };

        // Act
        var actual = priorExamMenu.GetMissingPriorMenus(unexaminedMenuIds);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    [Fact]
    public void 前提検査メニューのうち未受診の検査メニューがない()
    {
        // Arrange

        // 前提検査メニューの設定
        var currentExamMenuId = 6;
        var priorExamMenuIdList = new List<int>() { 1, 2, 3, 5 };
        var priorExamMenu = new PriorExamMenu(currentExamMenuId, priorExamMenuIdList);

        // 未受診の検査メニュー
        var unexaminedMenuIds = new List<int>() { 8, 9 };
        var expected = new List<int>();

        // Act
        var actual = priorExamMenu.GetMissingPriorMenus(unexaminedMenuIds);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
