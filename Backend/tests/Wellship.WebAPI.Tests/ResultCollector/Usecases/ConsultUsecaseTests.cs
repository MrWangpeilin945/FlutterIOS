using FluentAssertions;

using Moq;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
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

    [Fact]
    public async Task 未受診の検査メニューが存在する_未受診リストを返す()
    {
        // Arrange
        var consultNumber = "0001";
        var expectedUnexaminedMenus = new[]
        {
            new APIModels.Responses.ExamMenu { ExamMenuId = 1, ExamMenuName = "身体計測" },
            new APIModels.Responses.ExamMenu { ExamMenuId = 7, ExamMenuName = "血圧" }
        };

        var consultRepository = new Mock<IConsultRepository>();
        consultRepository.Setup(x => x.GetConsultAsync(consultNumber))
                         .ReturnsAsync(new Consult
                         {
                             ConsultId = 1,
                             ConsultNumber = "0001",
                             ExamineeId = 1,
                             ProgressStatus = ConsultProgressStatus.検査中,
                             ExportStatus = ConsultResultExportStatus.未出力,
                             PlaceScheduleId = 1
                         });

        consultRepository.Setup(x => x.GetUnexaminedConsultsAsync(new[] { consultNumber }))
                         .ReturnsAsync([
                            new(){
                                ConsultId = 1,
                                ConsultNumber = "0001",
                                ExamineeId = 1,
                                UnexaminedExamMenus = [
                                    new UnexaminedExamMenu(){
                                        ExamMenuId = 1,
                                        ExamMenuName = "身体計測"
                                    },
                                    new UnexaminedExamMenu(){
                                        ExamMenuId = 7,
                                        ExamMenuName = "血圧"
                                    }]}]);

        var examineeRepository = new Mock<IExamineeRepository>();
        examineeRepository.Setup(x => x.GetExamineeAsync(1))
                          .ReturnsAsync(new WebAPI.ResultCollector.Domain.Models.Examinee
                          {
                              ExamineeId = 1,
                              ExamineeCode = "10001",
                              Name = "両備　太郎",
                              KanaName = "リョウビ　タロウ",
                              Sex = Sex.男,
                              Birthdate = new Birthdate("19991129")
                          });

        var usecase = new ConsultUsecase(consultRepository.Object, examineeRepository.Object);

        // Act
        var result = await usecase.GetUnexaminedMenusAsync(consultNumber);

        // Assert
        result.Should().NotBeNull();
        result.UnexaminedMenus.Should().BeEquivalentTo(expectedUnexaminedMenus);
    }

    [Fact]
    public async Task 未受診の検査メニューが存在しない_空のリストを返す()
    {
        // Arrange
        var consultNumber = "yourConsultNumber";

        var consultRepository = new Mock<IConsultRepository>();
        consultRepository.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(new Consult()
        {
            ConsultNumber = "1",
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            PlaceScheduleId = 1,
            ConsultId = 1,
            ExamineeId = 1
        });
        var examineeRepository = new Mock<IExamineeRepository>();
        examineeRepository.Setup(x => x.GetExamineeAsync(1))
                          .ReturnsAsync(new WebAPI.ResultCollector.Domain.Models.Examinee
                          {
                              ExamineeId = 1,
                              ExamineeCode = "10001",
                              Name = "両備　太郎",
                              KanaName = "リョウビ　タロウ",
                              Sex = Sex.男,
                              Birthdate = new Birthdate("19991129")
                          });

        var usecase = new ConsultUsecase(consultRepository.Object, examineeRepository.Object);

        // Act
        var result = await usecase.GetUnexaminedMenusAsync(consultNumber);

        // Assert
        result.Should().NotBeNull();
        result.UnexaminedMenus.Should().BeEmpty();
    }
}