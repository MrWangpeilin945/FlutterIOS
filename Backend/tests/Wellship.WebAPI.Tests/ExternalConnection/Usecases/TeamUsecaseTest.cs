
using Moq;
using FluentAssertions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class TeamUsecaseTest
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly TimeProvider _timeProvider;

    public TeamUsecaseTest()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task 空値のチェック_班コードで1件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Code", InputNote  = "UT2006"}
        };
        var usecase = new TeamUsecase(_teamRepositoryMock.Object, _timeProvider);
        var request = new List<Team>
        {
            new Team { Code = "", Name = "一班", InputNote = "UT2006" }
        };
        // Act
        var errors = await usecase.StoreTeamsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }    

    [Fact]
    public async Task 空値のチェック_班名で1件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Name", InputNote  = "UT2006"}
        };
        var usecase = new TeamUsecase(_teamRepositoryMock.Object, _timeProvider);
        var request = new List<Team>
        {
            new Team { Code = "8931", Name = "", InputNote = "UT2006" }
        };
        // Act
        var errors = await usecase.StoreTeamsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 重複キーのチェック_班コードで2件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。Code:5963", InputNote  = "UT2006"},
            new(){Code = "10003", Message = "キー項目が重複しています。Code:5963", InputNote  = "UT2006"}
        };
        var usecase = new TeamUsecase(_teamRepositoryMock.Object, _timeProvider);
        var request = new List<Team>
        {
            new Team { Code = "5963", Name = "一班", InputNote = "UT2006" },
            new Team { Code = "5963", Name = "ニ班", InputNote = "UT2006" }
        };
        // Act
        var errors = await usecase.StoreTeamsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

}
