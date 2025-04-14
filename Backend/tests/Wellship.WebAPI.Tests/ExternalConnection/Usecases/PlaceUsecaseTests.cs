
using Moq;
using FluentAssertions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class PlaceUsecaseTests
{
    private readonly Mock<IPlaceRepository> _placeRepositoryMock;
    private readonly TimeProvider _timeProvider;

    public PlaceUsecaseTests()
    {
        _placeRepositoryMock = new Mock<IPlaceRepository>();
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task 空値のチェック_会場コードで1件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Code", InputNote  = "UT2007"}
        };
        var usecase = new PlaceUsecase(_placeRepositoryMock.Object, _timeProvider);
        var request = new List<Place>
        {
            new Place { Code = "", Name = "会場Ａ", InputNote = "UT2007" }
        };
        // Act
        var errors = await usecase.StorePlacesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_会場名で1件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Name", InputNote  = "UT2007"}
        };
        var usecase = new PlaceUsecase(_placeRepositoryMock.Object, _timeProvider);
        var request = new List<Place>
        {
            new Place { Code = "8931", Name = "", InputNote = "UT2007" }
        };
        // Act
        var errors = await usecase.StorePlacesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 重複キーのチェック_班コードで2件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。Code:5963", InputNote  = "UT2007"},
            new(){Code = "10003", Message = "キー項目が重複しています。Code:5963", InputNote  = "UT2007"}
        };
        var usecase = new PlaceUsecase(_placeRepositoryMock.Object, _timeProvider);
        var request = new List<Place>
        {
            new Place { Code = "5963", Name = "会場Ａ", InputNote = "UT2007" },
            new Place { Code = "5963", Name = "会場Ｂ", InputNote = "UT2007" }
        };
        // Act
        var errors = await usecase.StorePlacesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

}
