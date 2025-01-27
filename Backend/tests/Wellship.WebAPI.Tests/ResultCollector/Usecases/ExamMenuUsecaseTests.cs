using FluentAssertions;

using Moq;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class ExamMenuUsecaseTests
{
    [Fact]
    public async Task 検査メニューを取得してAPIレスポンス型に変換できる()
    {
        // Arrange
        var examMenuRepositoryMock = new Mock<IExamMenuRepository>();

        var examMenuData = new List<WebAPI.ResultCollector.Domain.Models.ExamMenu>() {
            new(){MenuId = 1,MenuName = "身体計測"},
            new(){MenuId = 2,MenuName = "視力"},
            new(){MenuId = 3,MenuName = "聴力"},
            new(){MenuId = 4,MenuName = "心電図"}
        };

        examMenuRepositoryMock.Setup(r => r.GetEnabledExamMenusAsync()).ReturnsAsync(examMenuData);
        var examMenuUsecase = new ExamMenuUsecase(examMenuRepositoryMock.Object);

        // ユースケースで変換後に期待するもの
        var expectedStaff = new ExamMenuList()
        {
            ExamMenus = [
                new ExamMenu() { ExamMenuId = 1, ExamMenuName = "身体計測" },
                new ExamMenu() { ExamMenuId = 2, ExamMenuName = "視力" },
                new ExamMenu() { ExamMenuId = 3, ExamMenuName = "聴力" },
                new ExamMenu() { ExamMenuId = 4, ExamMenuName = "心電図" }
            ]
        };

        // Act
        var result = await examMenuUsecase.GetExamMenusAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedStaff);
    }
}
