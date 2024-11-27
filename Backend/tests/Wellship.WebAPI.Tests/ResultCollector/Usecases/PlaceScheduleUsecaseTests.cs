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
                Id = 6,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 3,Code = "P003",Name = "会場C",OrderNumber = 3},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 3,Code = "T003",Name = "C班",OrderNumber = 3},
                ExamDate = new DateOnly(2024,11,20),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = 5,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 2,Code = "P002",Name = "会場B",OrderNumber = 2},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 3,Code = "T003",Name = "C班",OrderNumber = 3},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = 3,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 1,Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 2,Code = "T002",Name = "B班",OrderNumber = 2},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = 4,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 2,Code = "P002",Name = "会場B",OrderNumber = 2},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 2,Code = "T002",Name = "B班",OrderNumber = 2},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = 2,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 1,Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 1,Code = "T001",Name = "A班",OrderNumber = 1},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1300",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = 1,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 1,Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 1,Code = "T001",Name = "A班",OrderNumber = 1},
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
                    TeamId = 1,
                    TeamName = "A班",
                    Places = [
                        new APIModels.Responses.Place(){PlaceId = 1, PlaceName = "会場A"},
                    ]
                },
                new(){
                    TeamId = 2,
                    TeamName = "B班",
                    Places = [
                        new APIModels.Responses.Place(){PlaceId = 1, PlaceName = "会場A"},
                        new APIModels.Responses.Place(){PlaceId = 2, PlaceName = "会場B"}
                    ]
                },
                new(){
                    TeamId = 3,
                    TeamName = "C班",
                    Places = [
                        new APIModels.Responses.Place(){PlaceId = 2, PlaceName = "会場B"},
                        new APIModels.Responses.Place(){PlaceId = 3, PlaceName = "会場C"}
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
        var teamId = 1;

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
        var teamId = 1;

        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceSchedulesAsync(examDate))
                                    .ReturnsAsync(_placeSchedules);

        var expected = new APIModels.Responses.PlaceSchedulePlaces()
        {
            TeamId = 1,
            TeamName = "A班",
            ExamDate = new DateOnly(2024, 11, 30),
            PlaceSchedules = [
                new APIModels.Responses.PlaceSchedule(){
                    PlaceScheduleId = 1,
                    PlaceId = 1,
                    PlaceName = "会場A",
                    StartTime = "10:00"
                },
                new APIModels.Responses.PlaceSchedule(){
                    PlaceScheduleId = 2,
                    PlaceId = 1,
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
        var placeScheduleId = 1;

        var placeScheduleStatus = new PlaceScheduleStatus() {
            PlaceScheduleId = 1,
            PlaceId = 1, 
            PlaceName = "会場A",
            ExamDate = DateTime.Parse("2024-10-01"),
            Status = 21,
            CreatedAt = DateTime.Parse("2024-11-27 14:42:50"),
            CreatedBy = "tester"
        };
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleLockingStatusAsync(placeScheduleId))
                                    .ReturnsAsync(placeScheduleStatus);

        var expected = new APIModels.Responses.PlaceScheduleLocking()
        {
            PlaceScheduleId = 1,
            PlaceId = 1,
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
        var placeScheduleId = 1;
        var status = 11;
        _placeScheduleRepositoryMock.Setup(r => r.UpdatePlaceScheduleLockingStatusAsync(placeScheduleId, status));

        // Act & Assert
        await _placeScheduleUsecase.Invoking(x => x.UpdatePlaceScheduleLockingStatusAsync(placeScheduleId, status))
                                .Should().NotThrowAsync();
    }
}
