using FluentAssertions;

using Moq;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class CancelReasonUsecaseTests
{
    [Fact]
    public async Task 中止理由一覧を取得する_複数件()
    {
        // Arrange
        var expected = new APIModels.Responses.CancelReasonList()
        {
            CancelReasons =
            [
                new APIModels.Responses.CancelReason(){CancelReasonId = 1, CancelReasonName = "身体的理由", ExamItemId = 10},
                new APIModels.Responses.CancelReason(){CancelReasonId = 2, CancelReasonName = "中止理由A", ExamItemId = 3},
                new APIModels.Responses.CancelReason(){CancelReasonId = 4, CancelReasonName = "中止理由B", ExamItemId = 5},
            ]
        };


        var cancelReasonRepositoryMock = new Mock<ICancelReasonRepository>();
        cancelReasonRepositoryMock.Setup(x => x.GetCancelReasonsAsync())
                                  .ReturnsAsync([
                                    new CancelReason(){CancelReasonId = 1, Name = "身体的理由", ExamItemId = 10},
                                    new CancelReason(){CancelReasonId = 2, Name = "中止理由A", ExamItemId = 3},
                                    new CancelReason(){CancelReasonId = 4, Name = "中止理由B", ExamItemId = 5}
                                   ]);

        var usecase = new CancelReasonUsecase(cancelReasonRepositoryMock.Object);

        // Act
        var result = await usecase.GetCancelReasonsAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 中止理由一覧を取得する_0件()
    {
        // Arrange
        var expectedResult = new APIModels.Responses.CancelReasonList()
        {
            CancelReasons = []
        };

        var cancelReasonRepositoryMock = new Mock<ICancelReasonRepository>();
        cancelReasonRepositoryMock.Setup(x => x.GetCancelReasonsAsync())
                                  .ReturnsAsync([]);

        var usecase = new CancelReasonUsecase(cancelReasonRepositoryMock.Object);

        // Act
        var result = await usecase.GetCancelReasonsAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
