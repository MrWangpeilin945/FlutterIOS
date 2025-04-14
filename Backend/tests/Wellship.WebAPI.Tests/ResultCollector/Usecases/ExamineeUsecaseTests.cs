using FluentAssertions;

using Moq;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class ExamineeUsecaseTests
{
    private readonly Mock<IExamineeRepository> _examineeRepositoryMock;
    private readonly Mock<IPlaceScheduleRepository> _placeScheduleRepositoryMock;
    private readonly Mock<IResultRepository> _resultRepositoryMock;
    private readonly Mock<IProgressRepository> _progressRepositoryMock;
    private readonly Mock<IStaffIdentityProvider> _staffIdentityProviderMock;

    public ExamineeUsecaseTests()
    {
        _examineeRepositoryMock = new Mock<IExamineeRepository>();
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
        _resultRepositoryMock = new Mock<IResultRepository>();
        _progressRepositoryMock = new Mock<IProgressRepository>();
        _staffIdentityProviderMock = new Mock<IStaffIdentityProvider>();
    }

    [Fact]
    public async Task 受診者一覧を取得する()
    {
        // Arrange
        var placeScheduleId = Guid.NewGuid();
        var examMenuId = 1;
        var status = AggregatedProgressStatus.来場;
        Guid[] consultIds = new Guid[1];
        var examDate = DateOnly.FromDateTime(DateTime.Now);
        var checkedInAt = DateTimeOffset.Now;
        var AggregatedProgressDetail = new AggregatedProgressDetail
        {
            ExamMenuId = examMenuId,
            ExamMenuName = "身体計測",
            Count11 = 10,
            Count21 = 10,
            Count41 = 10,
            Count51 = 10
        };
        var PlaceSchedule = new PlaceSchedule
        {
            Id = placeScheduleId,
            Place = new Place
            {
                Id = Guid.Parse("aced0000-0000-0000-0000-000000000001"),
                Code = "P001",
                Name = "会場A",
                OrderNumber = 1
            },
            Team = new Team
            {
                Id = Guid.Parse("ea000000-0000-0000-0000-000000000001"),
                Code = "T001",
                Name = "班A",
                OrderNumber = 1
            },
            ExamDate = examDate,
            StartTime = "1300",
            PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
        };
        var ConsultExaminees = new ConsultExaminee
        {
            ConsultNumber = "0001",
            TicketNumber = "0001",
            KanaName = "リョウビ　タロウ",
            Sex = Sex.男,
            CheckedInAt = checkedInAt
        };

        _progressRepositoryMock.Setup(x => x.GetAggregatedProgressByMenuIdAsync(placeScheduleId, examMenuId))
                               .ReturnsAsync(AggregatedProgressDetail);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(placeScheduleId))
                                    .ReturnsAsync(PlaceSchedule);
        _examineeRepositoryMock.Setup(x => x.GetConsultIdsAsync(placeScheduleId, examMenuId, status))
                               .ReturnsAsync(consultIds);
        _examineeRepositoryMock.Setup(x => x.GetConsultExamineesAsync(consultIds))
                               .ReturnsAsync([ConsultExaminees]);

        var expected = new APIModels.Responses.ConsultExamineeList
        {
            PlaceScheduleId = placeScheduleId,
            PlaceName = "会場A",
            ExamDate = examDate,
            Progress = new APIModels.Responses.Progress()
            {
                ExamMenuId = examMenuId,
                ExamMenuName = "身体計測",
                Details = [
                    new APIModels.Responses.ProgressDetail(){Status = (int)AggregatedProgressStatus.予定 , StatusName = AggregatedProgressStatus.予定.ToString(), Count = AggregatedProgressDetail.Count11},
                    new APIModels.Responses.ProgressDetail(){Status = (int)AggregatedProgressStatus.来場 , StatusName = AggregatedProgressStatus.来場.ToString(), Count = AggregatedProgressDetail.Count21},
                    new APIModels.Responses.ProgressDetail(){Status = (int)AggregatedProgressStatus.済 , StatusName = AggregatedProgressStatus.済.ToString(), Count = AggregatedProgressDetail.Count41},
                    new APIModels.Responses.ProgressDetail(){Status = (int)AggregatedProgressStatus.中止 , StatusName = AggregatedProgressStatus.中止.ToString(), Count = AggregatedProgressDetail.Count51}
                ]
            },
            Examinees = [new APIModels.Responses.ConsultExaminee
            {
                ConsultNumber = "0001",
                TicketNumber = "0001",
                KanaName = "リョウビ　タロウ",
                Sex = (int)Sex.男,
                CheckedInAt = checkedInAt
            }]
        };

        var usecase = new ExamineeUsecase(_examineeRepositoryMock.Object, _progressRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

        // Act
        var result = await usecase.GetConsultExamineesAsync(placeScheduleId, examMenuId, status);

        // Assert
        result.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task 受診者一覧を取得する_未受付の場合()
    {
        // Arrange
        var placeScheduleId = Guid.NewGuid();
        var examMenuId = 1;
        var status = AggregatedProgressStatus.予定;
        Guid[] consultIds = new Guid[1];
        var examDate = DateOnly.FromDateTime(DateTime.Now);
        var AggregatedProgressDetail = new AggregatedProgressDetail
        {
            ExamMenuId = examMenuId,
            ExamMenuName = "身体計測",
            Count11 = 10,
            Count21 = 10,
            Count41 = 10,
            Count51 = 10
        };
        var PlaceSchedule = new PlaceSchedule
        {
            Id = placeScheduleId,
            Place = new Place
            {
                Id = Guid.Parse("aced0000-0000-0000-0000-000000000001"),
                Code = "P001",
                Name = "会場A",
                OrderNumber = 1
            },
            Team = new Team
            {
                Id = Guid.Parse("ea000000-0000-0000-0000-000000000001"),
                Code = "T001",
                Name = "班A",
                OrderNumber = 1
            },
            ExamDate = examDate,
            StartTime = "1300",
            PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
        };
        var ConsultExaminees = new ConsultExaminee
        {
            ConsultNumber = "0001",
            TicketNumber = null,
            KanaName = "リョウビ　タロウ",
            Sex = Sex.男,
            CheckedInAt = null
        };

        _progressRepositoryMock.Setup(x => x.GetAggregatedProgressByMenuIdAsync(placeScheduleId, examMenuId))
                                            .ReturnsAsync(AggregatedProgressDetail);
        _progressRepositoryMock.Setup(x => x.GetAggregatedProgressByMenuIdAsync(placeScheduleId, examMenuId))
                                            .ReturnsAsync(AggregatedProgressDetail);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(placeScheduleId))
                                                 .ReturnsAsync(PlaceSchedule);
        _examineeRepositoryMock.Setup(x => x.GetConsultIdsAsync(placeScheduleId, examMenuId, status))
                                            .ReturnsAsync(consultIds);
        _examineeRepositoryMock.Setup(x => x.GetConsultExamineesAsync(consultIds))
                                            .ReturnsAsync([ConsultExaminees]);

        var expected = new APIModels.Responses.ConsultExamineeList
        {
            PlaceScheduleId = placeScheduleId,
            PlaceName = "会場A",
            ExamDate = examDate,
            Progress = new APIModels.Responses.Progress()
            {
                ExamMenuId = examMenuId,
                ExamMenuName = "身体計測",
                Details = [
                    new APIModels.Responses.ProgressDetail(){Status = (int)AggregatedProgressStatus.予定 , StatusName = AggregatedProgressStatus.予定.ToString(), Count = AggregatedProgressDetail.Count11},
                    new APIModels.Responses.ProgressDetail(){Status = (int)AggregatedProgressStatus.来場 , StatusName = AggregatedProgressStatus.来場.ToString(), Count = AggregatedProgressDetail.Count21},
                    new APIModels.Responses.ProgressDetail(){Status = (int)AggregatedProgressStatus.済 , StatusName = AggregatedProgressStatus.済.ToString(), Count = AggregatedProgressDetail.Count41},
                    new APIModels.Responses.ProgressDetail(){Status = (int)AggregatedProgressStatus.中止 , StatusName = AggregatedProgressStatus.中止.ToString(), Count = AggregatedProgressDetail.Count51}
                ]
            },
            Examinees = [new APIModels.Responses.ConsultExaminee
            {
                ConsultNumber = "0001",
                TicketNumber = "",
                KanaName = "リョウビ　タロウ",
                Sex = (int)Sex.男,
                CheckedInAt = null
            }]
        };

        var usecase = new ExamineeUsecase(_examineeRepositoryMock.Object, _progressRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

        // Act
        var result = await usecase.GetConsultExamineesAsync(placeScheduleId, examMenuId, status);

        // Assert
        result.Should().BeEquivalentTo(result);
    }
}
