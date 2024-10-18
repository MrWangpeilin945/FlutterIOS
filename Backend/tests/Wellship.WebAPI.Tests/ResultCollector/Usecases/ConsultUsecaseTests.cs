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
    public void 受診番号が存在する場合に例外がスローされないこと()
    {
        // Arrange
        var consultationRepositoryMock = new Mock<IConsultRepository>();
        consultationRepositoryMock.Setup(x => x.ConsultExists(It.IsAny<string>())).Returns(true);

        var usecase = new ConsultUsecase(consultationRepositoryMock.Object);
        var request = new ConsultNumberRequest
        {
            ConsultNumber = "12345"
        };

        // Act & Assert
        usecase.Invoking(x => x.VerifyConsultNumber(request))
              .Should().NotThrow<ConsultNumberNotFoundException>();
    }

    [Fact]
    public void 受診番号が存在しない場合に例外がスローされること()
    {
        // Arrange
        var consultationRepositoryMock = new Mock<IConsultRepository>();
        consultationRepositoryMock.Setup(x => x.ConsultExists(It.IsAny<string>())).Returns(false);

        var usecase = new ConsultUsecase(consultationRepositoryMock.Object);
        var request = new ConsultNumberRequest
        {
            ConsultNumber = "54321"
        };

        // Act & Assert
        usecase.Invoking(x => x.VerifyConsultNumber(request))
              .Should().Throw<ConsultNumberNotFoundException>()
              .WithMessage("受診番号が存在しません。");
    }
}