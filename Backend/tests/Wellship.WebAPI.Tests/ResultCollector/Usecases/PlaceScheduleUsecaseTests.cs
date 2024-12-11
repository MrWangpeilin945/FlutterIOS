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
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("e9d78480-0e62-454b-9b23-0036b0e69bb1"),Code = "T003",Name = "C班",OrderNumber = 3},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = Guid.Parse("4eeb1061-8151-4e33-963b-bb2f06d9cb72"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("7e557f46-2df4-4317-848f-45df2bf5875d"),Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("278674a3-1aab-4e9c-bffc-b0ce88790a2d"),Code = "T002",Name = "B班",OrderNumber = 2},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = Guid.Parse("2a9eabe7-f240-4bef-a738-26a1d79ba698"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("19733ede-e8d2-4ef0-907e-4765cfd46686"),Code = "P002",Name = "会場B",OrderNumber = 2},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("0926c6a8-0e76-4163-aaea-1b65557064fd"),Code = "T002",Name = "B班",OrderNumber = 2},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = Guid.Parse("5eac0508-96eb-48ac-bf32-52dcd1776909"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("4e17c205-5b0a-4630-b7d1-037626947788"),Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("10064c79-ee76-43d5-bf26-d09dde5483f5"),Code = "T001",Name = "A班",OrderNumber = 1},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1300",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = Guid.Parse("b9d4d886-039e-49e5-9059-4b9f178ae57b"),
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = Guid.Parse("3579e83d-04ea-46fa-9529-d5f24537b48d"),Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = Guid.Parse("f1170138-dc6c-4771-855b-9de685ba35b7"),Code = "T001",Name = "A班",OrderNumber = 1},
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
                    TeamId = Guid.Parse("96d61460-4dc2-4442-840b-3ded7f32edcc"),
                    TeamName = "A班",
                    Places = [
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("da3db820-c84d-4999-89f0-943d110570a5"), PlaceName = "会場A"},
                    ]
                },
                new(){
                    TeamId = Guid.Parse("3c8a04f3-46d3-4b5a-ad7a-07ae07888bf1"),
                    TeamName = "B班",
                    Places = [
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("da3db820-c84d-4999-89f0-943d110570a5"), PlaceName = "会場A"},
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("adcfdc6d-e3f0-40d9-81ad-d12a3c532a65"), PlaceName = "会場B"}
                    ]
                },
                new(){
                    TeamId = Guid.Parse("0c651bfd-8ea3-4c83-acb4-833cc4153e62"),
                    TeamName = "C班",
                    Places = [
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("adcfdc6d-e3f0-40d9-81ad-d12a3c532a65"), PlaceName = "会場B"},
                        new APIModels.Responses.Place(){PlaceId = Guid.Parse("6ec90cea-8ce6-49c8-a1ad-5a6ef4d463c5"), PlaceName = "会場C"}
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
        var teamId = Guid.Parse("3d25f3cf-626f-4f9d-8e6d-693eb9c4607c");

        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceSchedulesAsync(examDate))
                                    .ReturnsAsync(_placeSchedules);

        var expected = new APIModels.Responses.PlaceSchedulePlaces()
        {
            TeamId = Guid.Parse("3d25f3cf-626f-4f9d-8e6d-693eb9c4607c"),
            TeamName = "A班",
            ExamDate = new DateOnly(2024, 11, 30),
            PlaceSchedules = [
                new APIModels.Responses.PlaceSchedule(){
                    PlaceScheduleId = Guid.Parse("c5217a6c-b7e2-431f-bfed-8d409fd013d6"),
                    PlaceId = Guid.Parse("34ac1ca1-5546-4915-91a3-bb34e73c8866"),
                    PlaceName = "会場A",
                    StartTime = "10:00"
                },
                new APIModels.Responses.PlaceSchedule(){
                    PlaceScheduleId = Guid.Parse("e7ff3df1-c0a2-4cbb-9286-fc5f15ee6a79"),
                    PlaceId = Guid.Parse("fafd3f2c-093e-4779-ae68-3869f9d4c55e"),
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
            Status = 21,
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
