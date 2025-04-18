using Moq;
using FluentAssertions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class TicketUsecaseTests
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly TimeProvider _timeProvider;

    public TicketUsecaseTests()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task 空値のチェック_連携キーで2件のエラーが返る()
    {
        // Arrange
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ConnectionCode", InputNote  = "UT2002"},
            new(){Code = "10001", Message = "指定されたConnectionCodeがシステム上に存在しません。Code:", InputNote  = "UT2002"}
        };
        var usecase = new TicketUsecase(_ticketRepositoryMock.Object, _timeProvider);
        var request = new List<Ticket>
        {
            new Ticket { SortNo = 1, ConnectionCode = "", ActionType = 0, TicketNumber = "T001", InputNote = "UT2002" }
        };
        // Act
        var errors = await usecase.StoreTicketsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_受付番号で1件のエラーが返る()
    {
        // Arrange
        // 外部キーに紐づく受診IDを取得する
        var request = new List<Ticket>
        {
            new Ticket { SortNo = 1, ConnectionCode = "2002001", ActionType = 0, TicketNumber = "", InputNote = "UT2002" }
        };
        var ticketsEntities = new List<TicketConsultEntity> {
                    new TicketConsultEntity(){ ConsultId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), ConnectionCode = "2002001"} };
        _ticketRepositoryMock.Setup(x => x.GetTicketsAsync(request)).ReturnsAsync(ticketsEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。TicketNumber", InputNote  = "UT2002"}
        };
        var usecase = new TicketUsecase(_ticketRepositoryMock.Object, _timeProvider);
        // Act
        var errors = await usecase.StoreTicketsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }    

    [Fact]
    public async Task 重複キーのチェック_処理順で2件のエラーが返る()
    {
        // Arrange
        // 外部キーに紐づく受診IDを取得する
        var request = new List<Ticket>
        {
            new Ticket { SortNo = 1, ConnectionCode = "2002001", ActionType = 0, TicketNumber = "T001", InputNote = "UT2002-01" },
            new Ticket { SortNo = 1, ConnectionCode = "2002002", ActionType = 0, TicketNumber = "T002", InputNote = "UT2002-02" }
        };
        var ticketsEntities = new List<TicketConsultEntity> {
                    new TicketConsultEntity(){ ConsultId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), ConnectionCode = "2002001"},
                    new TicketConsultEntity(){ ConsultId = Guid.Parse("a0000000-0000-0000-0000-000000000002"), ConnectionCode = "2002002"}
        };
        _ticketRepositoryMock.Setup(x => x.GetTicketsAsync(request)).ReturnsAsync(ticketsEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。SortNo:1", InputNote  = "UT2002-01"},
            new(){Code = "10003", Message = "キー項目が重複しています。SortNo:1", InputNote  = "UT2002-02"}
        };
        var usecase = new TicketUsecase(_ticketRepositoryMock.Object, _timeProvider);
        // Act
        var errors = await usecase.StoreTicketsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }    

    [Fact]
    public async Task データのチェック_連携キーで1件のエラーが返る()
    {
        // Arrange
        // 外部キーに紐づく受診IDを取得する
        var request = new List<Ticket>
        {
            new Ticket { SortNo = 1, ConnectionCode = "2002001", ActionType = 0, TicketNumber = "T001", InputNote = "UT2002" }
        };
        _ticketRepositoryMock.Setup(x => x.GetTicketsAsync(request)).ReturnsAsync(new List<TicketConsultEntity> {});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたConnectionCodeがシステム上に存在しません。Code:2002001", InputNote  = "UT2002"}
        };
        var usecase = new TicketUsecase(_ticketRepositoryMock.Object, _timeProvider);
        // Act
        var errors = await usecase.StoreTicketsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }    
}