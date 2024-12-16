using FluentAssertions;

using Moq;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class ProgressUsecaseTests
{
    [Fact]
    public async Task 進捗を取得する()
    {
        // Arrange
        var placeScheduleId = Guid.Parse("e1b9277a-c0a7-4de5-b36f-bc912e1189dd");

        var progressRepositoryMock = new Mock<IProgressRepository>();
        var placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();

        progressRepositoryMock.Setup(r => r.GetAggregatedProgressAsync(placeScheduleId))
                              .ReturnsAsync(new AggregatedProgress()
                              {
                                  PlaceScheduleId = Guid.Parse("e1b9277a-c0a7-4de5-b36f-bc912e1189dd"),
                                  AggregatedProgressDetails = [
                                    new AggregatedProgressDetail(){ExamItemId = 1, ExamItemName = "身長", Count11 = 10, Count21 = 8, Count41 = 0, Count51 = 0},
                                    new AggregatedProgressDetail(){ExamItemId = 2, ExamItemName = "体重", Count11 = 11, Count21 = 9, Count41 = 1, Count51 = 0},
                                    new AggregatedProgressDetail(){ExamItemId = 3, ExamItemName = "血圧", Count11 = 0, Count21 = 1, Count41 = 10, Count51 = 3}
                                  ]
                              });

        placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleAsync(placeScheduleId))
                                   .ReturnsAsync(new PlaceSchedule()
                                   {
                                       Id = Guid.Parse("e1b9277a-c0a7-4de5-b36f-bc912e1189dd"),
                                       Place = new() { Id = Guid.Parse("3236630c-9863-4e7b-88c5-530226a9375b"), Code = "P001", Name = "会場1", OrderNumber = 1 },
                                       Team = new() { Id = Guid.Parse("477cc2ca-e837-4f59-b8cc-ec351116f34d"), Code = "T001", Name = "班1", OrderNumber = 1 },
                                       ExamDate = new DateOnly(2024, 12, 3),
                                       StartTime = "0900",
                                       PlaceScheduleLockingStatus = Core.Enums.PlaceScheduleLockingStatus.検査中
                                   });

        var usecase = new ProgressUsecase(progressRepositoryMock.Object, placeScheduleRepositoryMock.Object);

        var expected = new APIModels.Responses.PlaceScheduleProgress()
        {
            PlaceScheduleId = Guid.Parse("e1b9277a-c0a7-4de5-b36f-bc912e1189dd"),
            PlaceName = "会場1",
            ExamDate = new DateOnly(2024, 12, 3),
            Progress = [
                new APIModels.Responses.Progress(){
                    ExamItemId = 1,
                    ExamItemName = "身長",
                    Details = [
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.予定, StatusName = Core.Enums.AggregatedProgressStatus.予定.ToString(), Count = 10},
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.来場, StatusName = Core.Enums.AggregatedProgressStatus.来場.ToString(), Count = 8},
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.済, StatusName = Core.Enums.AggregatedProgressStatus.済.ToString(), Count = 0},
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.中止, StatusName = Core.Enums.AggregatedProgressStatus.中止.ToString(), Count = 0}
                    ]
                },
                new APIModels.Responses.Progress(){
                    ExamItemId = 2,
                    ExamItemName = "体重",
                    Details = [
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.予定, StatusName = Core.Enums.AggregatedProgressStatus.予定.ToString(), Count = 11},
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.来場, StatusName = Core.Enums.AggregatedProgressStatus.来場.ToString(), Count = 9},
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.済, StatusName = Core.Enums.AggregatedProgressStatus.済.ToString(), Count = 1},
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.中止, StatusName = Core.Enums.AggregatedProgressStatus.中止.ToString(), Count = 0}
                    ]
                },
                new APIModels.Responses.Progress(){
                    ExamItemId = 3,
                    ExamItemName = "血圧",
                    Details = [
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.予定, StatusName = Core.Enums.AggregatedProgressStatus.予定.ToString(), Count = 0},
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.来場, StatusName = Core.Enums.AggregatedProgressStatus.来場.ToString(), Count = 1},
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.済, StatusName = Core.Enums.AggregatedProgressStatus.済.ToString(), Count = 10},
                        new (){Status = (int)Core.Enums.AggregatedProgressStatus.中止, StatusName = Core.Enums.AggregatedProgressStatus.中止.ToString(), Count = 3}
                    ]
                }
            ]
        };

        // Act
        var result = await usecase.GetProgressAsync(placeScheduleId);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
