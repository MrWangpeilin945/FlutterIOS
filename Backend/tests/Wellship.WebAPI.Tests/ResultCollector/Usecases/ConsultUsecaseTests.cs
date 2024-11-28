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
    public async Task 受診番号が存在する場合に例外がスローされない()
    {
        // Arrange
        var consultationRepositoryMock = new Mock<IConsultRepository>();
        consultationRepositoryMock.Setup(x => x.ConsultExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        var examineeRepositoryMock = new Mock<IExamineeRepository>();

        var usecase = new ConsultUsecase(consultationRepositoryMock.Object, examineeRepositoryMock.Object);
        var request = new ConsultNumberRequest
        {
            ConsultNumber = "12345"
        };

        // Act & Assert
        await usecase.VerifyConsultNumberAsync(request);
        await usecase.Invoking(x => x.VerifyConsultNumberAsync(request))
                      .Should().NotThrowAsync<ConsultNumberNotFoundException>();
    }

    [Fact]
    public async Task 受診番号が存在しない場合に例外がスローされる()
    {
        // Arrange
        var consultationRepositoryMock = new Mock<IConsultRepository>();
        consultationRepositoryMock.Setup(x => x.ConsultExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        var examineeRepositoryMock = new Mock<IExamineeRepository>();

        var usecase = new ConsultUsecase(consultationRepositoryMock.Object, examineeRepositoryMock.Object);
        var request = new ConsultNumberRequest
        {
            ConsultNumber = "54321"
        };

        // Act & Assert
        await usecase.Invoking(x => x.VerifyConsultNumberAsync(request))
                     .Should().ThrowAsync<ConsultNumberNotFoundException>()
                     .WithMessage("受診番号が存在しません。");
    }
}