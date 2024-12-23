using FluentAssertions;

using Moq;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class PlaceScheduleUsecaseTests
{
    private readonly Mock<IPlaceScheduleRepository> _placeScheduleRepositoryMock;
    private readonly PlaceScheduleUsecase _placeScheduleUsecase;
    private readonly List<WebAPI.ResultCollector.Domain.Models.PlaceSchedule> _placeSchedules;

    public PlaceScheduleUsecaseTests()
    {
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
        _placeScheduleUsecase = new PlaceScheduleUsecase(_placeScheduleRepositoryMock.Object);
        _placeSchedules =
        [
            new(){
                Id = Guid.Parse("7d112abd-079d-43d4-bc06-f652dbafb2f3"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("2d282b45-36ec-4c53-ad3a-53cbf3e67d2f"),Code = "P003",Name = "会場C",OrderNumber = 3},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("caf07515-7d93-41f2-b0c3-c2fe53f4e6e1"),Code = "T003",Name = "C班",OrderNumber = 3},
                ExamDate = new DateOnly(2024,11,20),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = Guid.Parse("e2c81d8c-26b3-46bd-873a-4ce5240f35bb"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("e51fb2b4-f53a-4e42-b984-9286ca28ebf3"),Code = "P002",Name = "会場B",OrderNumber = 2},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("caf07515-7d93-41f2-b0c3-c2fe53f4e6e1"),Code = "T003",Name = "C班",OrderNumber = 3},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = Guid.Parse("4eeb1061-8151-4e33-963b-bb2f06d9cb72"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("7e557f46-2df4-4317-848f-45df2bf5875d"),Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("e9d78480-0e62-454b-9b23-0036b0e69bb1"),Code = "T002",Name = "B班",OrderNumber = 2},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = Guid.Parse("2a9eabe7-f240-4bef-a738-26a1d79ba698"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("e51fb2b4-f53a-4e42-b984-9286ca28ebf3"),Code = "P002",Name = "会場B",OrderNumber = 2},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("e9d78480-0e62-454b-9b23-0036b0e69bb1"),Code = "T002",Name = "B班",OrderNumber = 2},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = Guid.Parse("5eac0508-96eb-48ac-bf32-52dcd1776909"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("7e557f46-2df4-4317-848f-45df2bf5875d"),Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("10064c79-ee76-43d5-bf26-d09dde5483f5"),Code = "T001",Name = "A班",OrderNumber = 1},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1300",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = Guid.Parse("b9d4d886-039e-49e5-9059-4b9f178ae57b"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("7e557f46-2df4-4317-848f-45df2bf5875d"),Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("10064c79-ee76-43d5-bf26-d09dde5483f5"),Code = "T001",Name = "A班",OrderNumber = 1},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            }
        ];
    }

    [Fact]
    public async Task 班で集約して会場日程を取得できる_空の場合()
    {
        // Arrange
        var examDate = new DateOnly(2024, 11, 1);
        var emptyList = new List<WebAPI.ResultCollector.Domain.Models.PlaceSchedule>();
        var placeSchedules = _placeScheduleRepositoryMock.Setup(x => x.GetPlaceSchedulesAsync(examDate))
                                                         .ReturnsAsync(emptyList);

        // Act
        var results = await _placeScheduleUsecase.GetTeamsAsync(examDate);

        // Assert
        results.Teams.Length.Should().Be(0);
    }

    [Fact]
    public async Task 班で集約して会場日程を取得できる_複数件の場合()
    {
        // Arrange
        var examDate = new DateOnly(2024, 11, 30);

        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceSchedulesAsync(examDate))
                                    .ReturnsAsync(_placeSchedules);

        var expected = new APIModels.Responses.PlaceScheduleTeams()
        {
            Teams =
            [
                new(){
                    TeamId = Guid.Parse("10064c79-ee76-43d5-bf26-d09dde5483f5"),
                    TeamName = "A班",
                    Places = [
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("7e557f46-2df4-4317-848f-45df2bf5875d"), PlaceName = "会場A"},
                    ]
                },
                new(){
                    TeamId = Guid.Parse("e9d78480-0e62-454b-9b23-0036b0e69bb1"),
                    TeamName = "B班",
                    Places = [
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("7e557f46-2df4-4317-848f-45df2bf5875d"), PlaceName = "会場A"},
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("e51fb2b4-f53a-4e42-b984-9286ca28ebf3"), PlaceName = "会場B"}
                    ]
                },
                new(){
                    TeamId = Guid.Parse("caf07515-7d93-41f2-b0c3-c2fe53f4e6e1"),
                    TeamName = "C班",
                    Places = [
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("e51fb2b4-f53a-4e42-b984-9286ca28ebf3"), PlaceName = "会場B"},
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("2d282b45-36ec-4c53-ad3a-53cbf3e67d2f"), PlaceName = "会場C"}
                    ]
                }
            ]
        };

        // Act
        var results = await _placeScheduleUsecase.GetTeamsAsync(examDate);

        // Assert
        results.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 班と健診日を指定して会場日程を取得できる_0件()
    {
        // Arrange
        var examDate = new DateOnly(2020, 11, 30);
        var teamId = Guid.Parse("6107813c-eae0-4f86-9096-861170419628");

        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceSchedulesAsync(examDate))
                                    .ReturnsAsync(_placeSchedules);

        // Act & Assert
        await _placeScheduleUsecase.Invoking(x => x.GetTeamPlaceSchedulesAsync(examDate, teamId))
                                   .Should().ThrowAsync<ResourceNotFoundException>()
                                   .WithMessage("会場日程が存在しません。");
    }

    [Fact]
    public async Task 班と健診日を指定して会場日程を取得できる_複数件()
    {
        // Arrange
        var examDate = new DateOnly(2024, 11, 30);
        var teamId = Guid.Parse("10064c79-ee76-43d5-bf26-d09dde5483f5");

        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceSchedulesAsync(examDate))
                                    .ReturnsAsync(_placeSchedules);

        var expected = new APIModels.Responses.PlaceSchedulePlaces()
        {
            TeamId = Guid.Parse("10064c79-ee76-43d5-bf26-d09dde5483f5"),
            TeamName = "A班",
            ExamDate = new DateOnly(2024, 11, 30),
            PlaceSchedules = [
                new APIModels.Responses.PlaceSchedule(){
                    PlaceScheduleId = Guid.Parse("b9d4d886-039e-49e5-9059-4b9f178ae57b"),
                    PlaceId = Guid.Parse("7e557f46-2df4-4317-848f-45df2bf5875d"),
                    PlaceName = "会場A",
                    StartTime = "10:00"
                },
                new APIModels.Responses.PlaceSchedule(){
                    PlaceScheduleId = Guid.Parse("5eac0508-96eb-48ac-bf32-52dcd1776909"),
                    PlaceId = Guid.Parse("7e557f46-2df4-4317-848f-45df2bf5875d"),
                    PlaceName = "会場A",
                    StartTime = "13:00"
                }
            ]
        };

        // Act
        var results = await _placeScheduleUsecase.GetTeamPlaceSchedulesAsync(examDate, teamId);

        // Assert
        results.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 会場ロック状態を取得する()
    {
        // Arrange
        var placeScheduleId = Guid.Parse("514486ae-acc7-40d8-8183-27c33963442b");

        var placeScheduleStatus = new PlaceScheduleStatus() {
            PlaceScheduleId = Guid.Parse("514486ae-acc7-40d8-8183-27c33963442b"),
            PlaceId = Guid.Parse("d60abfaa-fa22-4d37-9dcd-edf9fe5bd336"), 
            PlaceName = "会場A",
            ExamDate = DateTime.Parse("2024-10-01"),
            Status = PlaceScheduleLockingStatus.検査中,
            CreatedAt = DateTime.Parse("2024-11-27 14:42:50"),
            CreatedBy = "tester"
        };
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleLockingStatusAsync(placeScheduleId))
                                    .ReturnsAsync(placeScheduleStatus);

        var expected = new APIModels.Responses.PlaceScheduleLocking()
        {
            PlaceScheduleId = Guid.Parse("514486ae-acc7-40d8-8183-27c33963442b"),
            PlaceId = Guid.Parse("d60abfaa-fa22-4d37-9dcd-edf9fe5bd336"),
            PlaceName = "会場A",
            ExamDate = DateOnly.Parse("2024-10-01"),
            PlaceScheduleLockingStatus = 21,
            UpdatedAt = DateTime.Parse("2024-11-27 14:42:50"),
            UpdatedBy = "tester"
        };

        // Act
        var result = await _placeScheduleUsecase.GetPlaceScheduleLockingStatusAsync(placeScheduleId);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 会場ロック状態を更新する()
    {
        // Arrange
        var placeScheduleId = Guid.Parse("514486ae-acc7-40d8-8183-27c33963442b");
        PlaceScheduleLockingStatus status = PlaceScheduleLockingStatus.検査中;
        _placeScheduleRepositoryMock.Setup(r => r.UpdatePlaceScheduleLockingStatusAsync(placeScheduleId, status));

        // Act & Assert
        await _placeScheduleUsecase.Invoking(x => x.UpdatePlaceScheduleLockingStatusAsync(placeScheduleId, status))
                                .Should().NotThrowAsync();
    }
}
