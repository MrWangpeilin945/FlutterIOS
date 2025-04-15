
using Moq;
using FluentAssertions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class ThresholdUsecaseTests
{
    private readonly Mock<IThresholdRepository> _thresholdRepositoryMock;
    private readonly TimeProvider _timeProvider;
    public ThresholdUsecaseTests()
    {
        _thresholdRepositoryMock = new Mock<IThresholdRepository>();
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task 空値のチェック_基準値パターンコードで1件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Code", InputNote  = "UT2014"}
        };
        var usecase = new ThresholdUsecase(_thresholdRepositoryMock.Object, _timeProvider);
        var request = new List<Threshold>
        {
            new Threshold { Code = "", Name = "基準値パターン名", InputNote = "UT2014" }
        };
        // Act
        var errors = await usecase.StoreThresholdsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_基準値パターン名で1件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Name", InputNote  = "UT2014"}
        };
        var usecase = new ThresholdUsecase(_thresholdRepositoryMock.Object, _timeProvider);
        var request = new List<Threshold>
        {
            new Threshold { Code = "TH001", Name = "", InputNote = "UT2014" }
        };
        // Act
        var errors = await usecase.StoreThresholdsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 重複キーのチェック_基準値パターンコードで1件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。Code:TH001", InputNote  = "UT2014-01"},
            new(){Code = "10003", Message = "キー項目が重複しています。Code:TH001", InputNote  = "UT2014-02"}
        };
        var usecase = new ThresholdUsecase(_thresholdRepositoryMock.Object, _timeProvider);
        var request = new List<Threshold>
        {
            new Threshold { Code = "TH001", Name = "基準値パターンA", InputNote = "UT2014-01" },
            new Threshold { Code = "TH001", Name = "基準値パターンB", InputNote = "UT2014-02" }
        };
        // Act
        var errors = await usecase.StoreThresholdsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

}
