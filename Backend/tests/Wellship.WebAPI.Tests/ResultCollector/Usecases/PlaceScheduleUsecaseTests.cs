using FluentAssertions;

using Moq;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class PlaceScheduleUsecaseTests
{
    [Fact]
    public void 班でグループ化して会場日程を取得できる()
    {
        // Arrange
        var placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
        var usecase = new PlaceScheduleUsecase(placeScheduleRepositoryMock.Object);

        var date = new DateOnly(2024, 09, 27);
        var placeSchedules = new[]
        {
            new PlaceSchedule(1, new WebAPI.ResultCollector.Domain.Models.Place(10, "豊成オフィス"), new Team(2, "2班"), new DateOnly(2024, 09, 27), "09:00", "12:00"),
            new PlaceSchedule(3, new WebAPI.ResultCollector.Domain.Models.Place(10, "豊成オフィス"), new Team(3, "3班"), new DateOnly(2024, 09, 27), "09:00", "12:00"),
            new PlaceSchedule(4, new WebAPI.ResultCollector.Domain.Models.Place(20, "藤崎オフィス"), new Team(2, "2班"), new DateOnly(2024, 09, 27), "13:00", "17:00")
        };

        placeScheduleRepositoryMock.Setup(x => x.GetPlaceSchedules(date)).Returns(placeSchedules);

        // Act
        var result = usecase.GetTeams(date);

        // Assert
        var expectedTeams = new[]
        {
                new PlaceScheduleTeam(2, "2班",
                [
                    new APIModels.Responses.Place(10, "豊成オフィス"),
                    new APIModels.Responses.Place(20, "藤崎オフィス")
                ]),
                new PlaceScheduleTeam(3, "3班",
                [
                    new APIModels.Responses.Place(10, "豊成オフィス")
                ])
            };
        result.Teams.Should().BeEquivalentTo(expectedTeams);
    }
}
