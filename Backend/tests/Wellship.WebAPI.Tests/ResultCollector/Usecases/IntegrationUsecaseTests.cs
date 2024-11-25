using FluentAssertions;

using Moq;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class IntegrationUsecaseTests
{

    private readonly Mock<IPlaceScheduleRepository> _placeScheduleRepositoryMock;
    private readonly Mock<IIntegrationRepository> _integrationRepositoryMock;
    private readonly List<WebAPI.ResultCollector.Domain.Models.PlaceSchedule> _placeSchedules;
    private readonly List<WebAPI.ResultCollector.Domain.Models.ExportHistory> _exportHistories;

    public IntegrationUsecaseTests()
    {
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
        _integrationRepositoryMock = new Mock<IIntegrationRepository>();

        _placeSchedules =
        [
            new(){
                Id = 3,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 1,Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 2,Code = "T002",Name = "B班",OrderNumber = 2},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査完了
            },
            new(){
                Id = 4,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 2,Code = "P002",Name = "会場B",OrderNumber = 2},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 2,Code = "T002",Name = "B班",OrderNumber = 2},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
            },
            new(){
                Id = 2,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 1,Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 1,Code = "T001",Name = "A班",OrderNumber = 1},
                ExamDate = new DateOnly(2024,10,21),
                StartTime = "1300",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査完了
            },
            new(){
                Id = 1,
                Place = new WebAPI.ResultCollector.Domain.Models.Place(){Id = 1,Code = "P001",Name = "会場A",OrderNumber = 1},
                Team = new WebAPI.ResultCollector.Domain.Models.Team(){Id = 1,Code = "T001",Name = "A班",OrderNumber = 1},
                ExamDate = new DateOnly(2024,11,30),
                StartTime = "1000",
                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査完了
            }
        ];

        _exportHistories = [
            new(){PlaceScheduleId = 1,DataCount = 13,ExportedAt = DateTime.Parse("2024-11-30 17:00"),ExportedBy = "職員A"},
            new(){PlaceScheduleId = 1,DataCount = 45,ExportedAt = DateTime.Parse("2024-11-30 13:00"),ExportedBy = "職員B"},
            new(){PlaceScheduleId = 2,DataCount = 2,ExportedAt = DateTime.Parse("2024-10-21 17:30"),ExportedBy = "職員B"}
        ];
    }

    [Fact]
    public async Task 検査結果出力履歴を取得できる_複数件()
    {
        // Arrange

        _integrationRepositoryMock.Setup(r => r.GetExportHistoryAsync()).ReturnsAsync(_exportHistories);
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceSchedulesAsync(new int[] { 1, 2 })).ReturnsAsync(_placeSchedules);

        var integrationUsecase = new IntegrationUsecase(_integrationRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

        // ユースケースで変換後に期待するもの
        // 出力日時の降順であること
        var expected = new APIModels.Responses.ExportHistoryList()
        {
            ExportHistories = [
                new(){
                    PlaceScheduleId = 1,
                    PlaceName = "会場A",
                    PlaceScheduleLockingStatus = (int)PlaceScheduleLockingStatus.検査完了,
                    ExamDate = DateOnly.Parse("2024-11-30"),
                    DataCount = 13,
                    ExportedAt = DateTime.Parse("2024-11-30 17:00"),
                    ExportedBy = "職員A"
                },
                new(){
                    PlaceScheduleId = 1,
                    PlaceName = "会場A",
                    PlaceScheduleLockingStatus = (int)PlaceScheduleLockingStatus.検査完了,
                    ExamDate = DateOnly.Parse("2024-11-30"),
                    DataCount = 45,
                    ExportedAt = DateTime.Parse("2024-11-30 13:00"),
                    ExportedBy = "職員B"
                },
                new(){
                    PlaceScheduleId = 2,
                    PlaceName = "会場A",
                    PlaceScheduleLockingStatus = (int)PlaceScheduleLockingStatus.検査完了,
                    ExamDate = DateOnly.Parse("2024-10-21"),
                    DataCount = 2,
                    ExportedAt = DateTime.Parse("2024-10-21 17:30"),
                    ExportedBy = "職員B"
                }
            ]
        };

        // Act
        var result = await integrationUsecase.GetExportHistoryAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 検査結果出力履歴を取得できる_0件()
    {
        // Arrange

        _integrationRepositoryMock.Setup(r => r.GetExportHistoryAsync()).ReturnsAsync([]);
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceSchedulesAsync(new int[] { })).ReturnsAsync([]);

        var integrationUsecase = new IntegrationUsecase(_integrationRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

        // ユースケースで変換後に期待するもの
        var expected = new APIModels.Responses.ExportHistoryList()
        {
            ExportHistories = []
        };

        // Act
        var result = await integrationUsecase.GetExportHistoryAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
