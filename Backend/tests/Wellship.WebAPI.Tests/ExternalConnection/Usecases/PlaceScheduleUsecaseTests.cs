using Moq;
using FluentAssertions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class PlaceScheduleUsecaseTests
{
    private readonly Mock<IPlaceScheduleRepository> _placeScheduleRepositoryMock;
    private readonly Mock<IPlaceRepository> _placeRepositoryMock;
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly TimeProvider _timeProvider;

    public PlaceScheduleUsecaseTests()
    {
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
        _placeRepositoryMock = new Mock<IPlaceRepository>();
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task 空値のチェック_班コードで2件のエラーが返る()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(x => x.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        _teamRepositoryMock.Setup(x => x.GetTeamInfoAsync(new List<string> { "" })).ReturnsAsync(new List<TeamEntity>());

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。TeamCode", InputNote  = "UT2012"},
            new(){Code = "10001", Message = "指定されたTeamCodeがシステム上に存在しません。Code:", InputNote  = "UT2012"}
        };
        var usecase = new PlaceScheduleUsecase(_placeScheduleRepositoryMock.Object,
                                               _placeRepositoryMock.Object, _teamRepositoryMock.Object, _timeProvider);
        var request = new List<PlaceSchedule>
        {
            new PlaceSchedule { TeamCode = "", PlaceCode = "P001", ExamDate = DateTime.Parse("2025-02-02T09:30:00"), InputNote = "UT2012" }
        };
        // Act
        var errors = await usecase.StorePlaceSchedulesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_会場コードで2件のエラーが返る()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        _placeRepositoryMock.Setup(x => x.GetPlaceInfoAsync(new List<string> { "" })).ReturnsAsync(new List<PlaceEntity>());
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(x => x.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。PlaceCode", InputNote  = "UT2012"},
            new(){Code = "10001", Message = "指定されたPlaceCodeがシステム上に存在しません。Code:", InputNote  = "UT2012"}
        };
        var usecase = new PlaceScheduleUsecase(_placeScheduleRepositoryMock.Object,
                                               _placeRepositoryMock.Object, _teamRepositoryMock.Object, _timeProvider);
        var request = new List<PlaceSchedule>
        {
            new PlaceSchedule { TeamCode = "T001", PlaceCode = "", ExamDate = DateTime.Parse("2025-02-02T09:30:00"), InputNote = "UT2012" }
        };
        // Act
        var errors = await usecase.StorePlaceSchedulesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 重複キーのチェック_会場コードと班コードと健診日で2件のエラーが返る()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(x => x.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(x => x.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。TeamCode:T001/PlaceCode:P001/ExamDate:2025/04/30", InputNote  = "UT2012-1"},
            new(){Code = "10003", Message = "キー項目が重複しています。TeamCode:T001/PlaceCode:P001/ExamDate:2025/04/30", InputNote  = "UT2012-2"}
        };
        var usecase = new PlaceScheduleUsecase(_placeScheduleRepositoryMock.Object,
                                               _placeRepositoryMock.Object, _teamRepositoryMock.Object, _timeProvider);
        var request = new List<PlaceSchedule>
        {
            new PlaceSchedule { TeamCode = "T001", PlaceCode = "P001", ExamDate = DateTime.Parse("2025-04-30T00:30:00"), InputNote = "UT2012-1" },
            new PlaceSchedule { TeamCode = "T001", PlaceCode = "P001", ExamDate = DateTime.Parse("2025-04-30T22:50:30"), InputNote = "UT2012-2" }
        };
        // Act
        var errors = await usecase.StorePlaceSchedulesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task データのチェック_会場IDで1件のエラーが返る()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        _placeRepositoryMock.Setup(x => x.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(new List<PlaceEntity>());
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(x => x.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたPlaceCodeがシステム上に存在しません。Code:P001", InputNote  = "UT2012"}
        };
        var usecase = new PlaceScheduleUsecase(_placeScheduleRepositoryMock.Object,
                                               _placeRepositoryMock.Object, _teamRepositoryMock.Object, _timeProvider);
        var request = new List<PlaceSchedule>
        {
            new PlaceSchedule { TeamCode = "T001", PlaceCode = "P001", ExamDate = DateTime.Parse("2025-04-30T00:30:00"), InputNote = "UT2012" }
        };
        // Act
        var errors = await usecase.StorePlaceSchedulesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task データのチェック_班IDで1件のエラーが返る()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(x => x.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        _teamRepositoryMock.Setup(x => x.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(new List<TeamEntity>());

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたTeamCodeがシステム上に存在しません。Code:T001", InputNote  = "UT2012"}
        };
        var usecase = new PlaceScheduleUsecase(_placeScheduleRepositoryMock.Object,
                                               _placeRepositoryMock.Object, _teamRepositoryMock.Object, _timeProvider);
        var request = new List<PlaceSchedule>
        {
            new PlaceSchedule { TeamCode = "T001", PlaceCode = "P001", ExamDate = DateTime.Parse("2025-04-30T00:30:00"), InputNote = "UT2012" }
        };
        // Act
        var errors = await usecase.StorePlaceSchedulesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }    

}