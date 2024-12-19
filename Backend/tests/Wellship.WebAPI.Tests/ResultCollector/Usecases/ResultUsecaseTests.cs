using FluentAssertions;

using Moq;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.Core.Exceptions;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class ResultUsecaseTests
{
    private readonly Mock<IResultRepository> _resultRepositoryMock;
    private readonly Mock<IConsultRepository> _consultRepositoryMock;
    private readonly Mock<IPlaceScheduleRepository> _placeScheduleRepositoryMock;

    public ResultUsecaseTests()
    {
        _resultRepositoryMock = new Mock<IResultRepository>();
        _consultRepositoryMock = new Mock<IConsultRepository>();
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
    }

    [Fact]
    public async Task 検査結果を登録する()
    {
        // Arrange
        var consultNumber = "0006";
        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber))
                         .ReturnsAsync(new Consult
                         {
                             ConsultId = Guid.Parse("8d670eb8-d9f2-40b2-bfa6-cef6403be53d"),
                             ConsultNumber = "0006",
                             ExamineeId = Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3"),
                             ProgressStatus = ConsultProgressStatus.検査中,
                             Note = "定期健康診断",
                             ExportStatus = ConsultResultExportStatus.未出力,
                             PlaceScheduleId = Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250"),
                             TicketNumber = "1029"
                         });
        var usecase = new ResultUsecase(_resultRepositoryMock.Object, _consultRepositoryMock.Object, _placeScheduleRepositoryMock.Object);
        var results = new ResultsRequest
        {
            ExamMenuId = 7,
            ExamResults = [
                new ResultRequest()
                {
                    ExamItemId = 71
                    , ExamItemDetails = 
                    [ 
                        new ExamItemDetailRequest(){ExamItemDetailId = 711, Value = "100"},
                        new ExamItemDetailRequest(){ExamItemDetailId = 712, Value = "80"}
                    ]
                },
                new ResultRequest() {
                    ExamItemId = 72
                    , ExamItemDetails = 
                    [ 
                        new ExamItemDetailRequest(){ExamItemDetailId = 721, Value = "110"},
                        new ExamItemDetailRequest(){ExamItemDetailId = 722, Value = "95"}
                    ]
                }
            ]
        };
        // Act & Assert
        await usecase.RegisterResultsAsync(consultNumber, results);
    }
    [Fact]
    public async Task 会場ロック状態で検査結果を登録する()
    {
        // Arrange
        var consultNumber = "0006";
        var placeScheduleId = Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250");
        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber))
                            .ReturnsAsync(new Consult
                            {
                                ConsultId = Guid.Parse("8d670eb8-d9f2-40b2-bfa6-cef6403be53d"),
                                ConsultNumber = "0006",
                                ExamineeId = Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3"),
                                ProgressStatus = ConsultProgressStatus.検査中,
                                Note = "定期健康診断",
                                ExportStatus = ConsultResultExportStatus.未出力,
                                PlaceScheduleId = placeScheduleId,
                                TicketNumber = "1029"
                            });
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleLockingStatusAsync(placeScheduleId))
                                    .ReturnsAsync(new PlaceScheduleStatus
                                    {
                                        PlaceScheduleId = placeScheduleId,
                                        PlaceId = Guid.Parse("492d6d5c-17ab-4aba-89e2-51369373b8a8"),
                                        PlaceName = "市役所",
                                        ExamDate = DateTime.Parse("2024-12-19"),
                                        Status = PlaceScheduleLockingStatus.検査完了,
                                        CreatedAt = DateTime.Now,
                                        CreatedBy = "admin"
                                    });     
        var usecase = new ResultUsecase(_resultRepositoryMock.Object, _consultRepositoryMock.Object, _placeScheduleRepositoryMock.Object);
        var results = new ResultsRequest
        {
            ExamMenuId = 7,
            ExamResults = [
                new ResultRequest()
                {
                    ExamItemId = 71
                    , ExamItemDetails = 
                    [ 
                        new ExamItemDetailRequest(){ExamItemDetailId = 711, Value = "100"},
                        new ExamItemDetailRequest(){ExamItemDetailId = 712, Value = "80"}
                    ]
                },
                new ResultRequest() {
                    ExamItemId = 72
                    , ExamItemDetails = 
                    [ 
                        new ExamItemDetailRequest(){ExamItemDetailId = 721, Value = "110"},
                        new ExamItemDetailRequest(){ExamItemDetailId = 722, Value = "95"}
                    ]
                }
            ]
        };
        // Act & Assert
        await usecase.Invoking(x => x.RegisterResultsAsync(consultNumber, results))
                     .Should().ThrowAsync<PlaceScheduleLockedException>();
    }

}
