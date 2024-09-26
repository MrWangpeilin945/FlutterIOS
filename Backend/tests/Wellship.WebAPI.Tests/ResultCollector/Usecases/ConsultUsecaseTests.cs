using FluentAssertions;

using Moq;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class ConsultUsecaseTests
{
    [Fact]
    public void 予約Noが存在する場合に例外がスローされないこと()
    {
        // Arrange
        var consultationRepositoryMock = new Mock<IConsultRepository>();
        consultationRepositoryMock.Setup(x => x.ConsultExists(It.IsAny<string>())).Returns(true);

        var usecase = new ConsultUsecase(consultationRepositoryMock.Object);
        var request = new ReservationNoRequest
        {
            ReservationNo = "12345"
        };

        // Act & Assert
        usecase.Invoking(x => x.VerifyReservationNo(request))
              .Should().NotThrow<ReservationNoNotFoundException>();
    }

    [Fact]
    public void 予約Noが存在しない場合に例外がスローされること()
    {
        // Arrange
        var consultationRepositoryMock = new Mock<IConsultRepository>();
        consultationRepositoryMock.Setup(x => x.ConsultExists(It.IsAny<string>())).Returns(false);

        var usecase = new ConsultUsecase(consultationRepositoryMock.Object);
        var request = new ReservationNoRequest
        {
            ReservationNo = "54321"
        };

        // Act & Assert
        usecase.Invoking(x => x.VerifyReservationNo(request))
              .Should().Throw<ReservationNoNotFoundException>()
              .WithMessage("予約Noが存在しません。");
    }
}