using FluentAssertions;

using Moq;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Auth;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class ConsultUsecaseTests
{
    private readonly Mock<IExamineeRepository> _examineeRepositoryMock;
    private readonly Mock<IConsultRepository> _consultRepositoryMock;
    private readonly Mock<IExamMenuRepository> _examMenuRepositoryMock;
    private readonly Mock<IExamItemRepository> _examItemRepositoryMock;
    private readonly Mock<IPlaceScheduleRepository> _placeScheduleRepositoryMock;
    private readonly Mock<IResultRepository> _resultRepositoryMock;
    private readonly Mock<IStaffIdentityProvider> _staffIdentityProviderMock;

    public ConsultUsecaseTests()
    {
        _examineeRepositoryMock = new Mock<IExamineeRepository>();
        _consultRepositoryMock = new Mock<IConsultRepository>();
        _examMenuRepositoryMock = new Mock<IExamMenuRepository>();
        _examItemRepositoryMock = new Mock<IExamItemRepository>();
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
        _resultRepositoryMock = new Mock<IResultRepository>();
        _staffIdentityProviderMock = new Mock<IStaffIdentityProvider>();
    }

    [Fact]
    public async Task 受診番号が存在する場合に例外がスローされない()
    {
        // Arrange
        _consultRepositoryMock.Setup(x => x.ConsultExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);
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
        _consultRepositoryMock.Setup(x => x.ConsultExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);
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

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber))
                         .ReturnsAsync(new Consult
                         {
                             ConsultId = Guid.Parse("8d670eb8-d9f2-40b2-bfa6-cef6403be53d"),
                             ConsultNumber = "0001",
                             Age = new Age(25, 0, 0),
                             ExamineeId = Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3"),
                             ProgressStatus = ConsultProgressStatus.検査中,
                             Note = "定期健康診断",
                             ExportStatus = ConsultResultExportStatus.未出力,
                             PlaceScheduleId = Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250"),
                             TicketNumber = "1029"
                         });

        _consultRepositoryMock.Setup(x => x.GetUnexaminedConsultsAsync(new[] { consultNumber }))
                         .ReturnsAsync([
                            new(){
                                ConsultId = Guid.Parse("8d670eb8-d9f2-40b2-bfa6-cef6403be53d"),
                                ConsultNumber = "0001",
                                ExamineeId = Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3"),
                                UnexaminedExamMenus = [
                                    new UnexaminedExamMenu(){
                                        ExamMenuId = 1,
                                        ExamMenuName = "身体計測"
                                    },
                                    new UnexaminedExamMenu(){
                                        ExamMenuId = 7,
                                        ExamMenuName = "血圧"
                                    }]}]);

        _examineeRepositoryMock.Setup(x => x.GetExamineeAsync(Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3")))
                          .ReturnsAsync(new WebAPI.ResultCollector.Domain.Models.Examinee
                          {
                              ExamineeId = Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3"),
                              ExamineeCode = "10001",
                              Name = "両備　太郎",
                              KanaName = "リョウビ　タロウ",
                              Sex = Sex.男,
                              Birthdate = new Birthdate("19991129"),
                              Affiliations = [new Affiliations
                                {
                                    OrganizationId = Guid.Parse("7bf76a3d-8bb4-41f5-8bd5-3b8fc8482511"),
                                    OrganizationCode = "0001",
                                    OrganizationName = "",
                                    OrderNumber = 1
                                }]
                          });

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

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

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(new Consult()
        {
            ConsultNumber = "1",
            Age = new Age(0, 0, 0),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            PlaceScheduleId = Guid.Parse("6df8bdff-66c0-4113-afd6-985a78bd35ff"),
            Note = "定期健康診断",
            ConsultId = Guid.Parse("0c48b92e-7b65-4e39-8d19-7dcdead5a763"),
            ExamineeId = Guid.Parse("85417626-3b52-44f1-82cc-2bad8e43d5df"),
            TicketNumber = "1029"
        });
        _examineeRepositoryMock.Setup(x => x.GetExamineeAsync(Guid.Parse("85417626-3b52-44f1-82cc-2bad8e43d5df")))
                          .ReturnsAsync(new WebAPI.ResultCollector.Domain.Models.Examinee
                          {
                              ExamineeId = Guid.Parse("85417626-3b52-44f1-82cc-2bad8e43d5df"),
                              ExamineeCode = "10001",
                              Name = "両備　太郎",
                              KanaName = "リョウビ　タロウ",
                              Sex = Sex.男,
                              Birthdate = new Birthdate("19991129"),
                              Affiliations = [new Affiliations
                                {
                                    OrganizationId = Guid.Parse("7bf76a3d-8bb4-41f5-8bd5-3b8fc8482511"),
                                    OrganizationCode = "0001",
                                    OrganizationName = "",
                                    OrderNumber = 1
                                }]
                          });

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act
        var result = await usecase.GetUnexaminedMenusAsync(consultNumber);

        // Assert
        result.Should().NotBeNull();
        result.UnexaminedMenus.Should().BeEmpty();
    }

    [Fact]
    public async Task 検査の実施有無と中止理由を登録する_検査実施する中止理由なし()
    {
        // NOTE: ロジックは条件式やSQLで書いているため、このテストコードにあまり意味はありません。
        // 代表的な動作例を書いています。

        // Arrange

        // リクエスト内容
        var consultNumber = "12345";
        var request = new ExecutionsRequest()
        {
            // すべて実施する
            Executions = [
                new ExecutionRequest() {ExamItemId = 1, IsPerforming = true, CancelReasonId = null},
                new ExecutionRequest() {ExamItemId = 2, IsPerforming = true, CancelReasonId = null},
                new ExecutionRequest() {ExamItemId = 71, IsPerforming = true, CancelReasonId = null},
                new ExecutionRequest() {ExamItemId = 72, IsPerforming = true, CancelReasonId = null}
            ]
        };

        // DBの受診情報
        var consult = new Consult()
        {
            ConsultId = Guid.Parse("7400c9cc-c2ee-48ef-a12e-5c3244631a6d"),
            ConsultNumber = "0001",
            Age = new Age(0, 0, 0),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            PlaceScheduleId = Guid.Parse("caab7279-109a-4c63-bc9a-83fa25c59a91"),
            Note = "定期健康診断",
            ExamineeId = Guid.Parse("9944b7f3-0728-4801-853f-424a9c23a0b9"),
            TicketNumber = "1029"
        };

        // すでに存在するキャンセルレコード
        var examCancels = new ExamCancel()
        {
            ConsultId = Guid.Parse("ea0729d2-30c6-46e8-8fef-e73a666b71cd"),
            ExamItemDetailCancels = [
                new ExamItemDetailCancel() {ExamItemDetailId = 1, ExamItemId = 1, CancelReasonId = 1},
                new ExamItemDetailCancel() {ExamItemDetailId = 711, ExamItemId = 71, CancelReasonId = 3},
                new ExamItemDetailCancel() {ExamItemDetailId = 712, ExamItemId = 71, CancelReasonId = 3},
            ]
        };

        // 会場日程ステータス
        var placeScheduleStatus = new PlaceScheduleStatus
        {
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            PlaceId = Guid.Parse("492d6d5c-17ab-4aba-89e2-51369373b8a8"),
            PlaceName = "市役所",
            ExamDate = new DateOnly(2024, 12, 19),
            Status = PlaceScheduleLockingStatus.検査中,
            CreatedAt = DateTime.Now,
            CreatedBy = "admin"
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consult.ConsultId)).ReturnsAsync(examCancels);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleLockingStatusAsync(It.IsAny<Guid>()))
                                    .ReturnsAsync(placeScheduleStatus);
        _staffIdentityProviderMock.Setup(x => x.Role).Returns(Role.User);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act
        await usecase.RegisterExecutionsAsync(consultNumber, request);

        // Assert
        _consultRepositoryMock.Verify(x => x.SaveExamCancelsAsync(consult.ConsultId, It.IsAny<int[]>(), It.IsAny<ExamItemCancel[]>()), Times.Once);
    }

    [Fact]
    public async Task 検査の実施有無と中止理由を登録する_検査実施しない中止理由あり()
    {
        // NOTE: ロジックは条件式やSQLで書いているため、このテストコードにあまり意味はありません。
        // 代表的な動作例を書いています。

        // Arrange

        // リクエスト内容
        var consultNumber = "12345";
        var request = new ExecutionsRequest()
        {
            Executions = [
                new ExecutionRequest(){ExamItemId = 1, IsPerforming = false, CancelReasonId = 1}
            ]
        };

        // DBの受診情報
        var consult = new Consult()
        {
            ConsultId = Guid.Parse("4a778828-c758-4437-99ec-3493310edf46"),
            ConsultNumber = "0001",
            Age = new Age(0, 0, 0),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            PlaceScheduleId = Guid.Parse("ed3e8d23-2c9f-4b0c-bfb1-e933afc8ee49"),
            Note = "定期健康診断",
            ExamineeId = Guid.Parse("9409a9a9-163e-4721-a1ae-9fa83017bcf7"),
            TicketNumber = "1029"
        };

        // すでに存在するキャンセルレコード
        var examCancel = new ExamCancel()
        {
            // キャンセルレコードなし
            ConsultId = Guid.Parse("4075747f-579b-4fc0-ad8e-4ef1c0fc6c9f"),
            ExamItemDetailCancels = []
        };

        // 会場日程ステータス
        var placeScheduleStatus = new PlaceScheduleStatus
        {
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            PlaceId = Guid.Parse("492d6d5c-17ab-4aba-89e2-51369373b8a8"),
            PlaceName = "市役所",
            ExamDate = new DateOnly(2024, 12, 19),
            Status = PlaceScheduleLockingStatus.検査完了,
            CreatedAt = DateTime.Now,
            CreatedBy = "admin"
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consult.ConsultId)).ReturnsAsync(examCancel);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleLockingStatusAsync(It.IsAny<Guid>()))
                                    .ReturnsAsync(placeScheduleStatus);
        _staffIdentityProviderMock.Setup(x => x.Role).Returns(Role.Admin);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act
        await usecase.RegisterExecutionsAsync(consultNumber, request);

        // Assert
        _consultRepositoryMock.Verify(x => x.SaveExamCancelsAsync(consult.ConsultId, It.IsAny<int[]>(), It.IsAny<ExamItemCancel[]>()), Times.Once);
    }

    [Fact]
    public async Task 検査の実施有無と中止理由を登録する_管理者でない時にエラーを送出()
    {
        // Arrange
        // リクエスト内容
        var consultNumber = "12345";
        var request = new ExecutionsRequest()
        {
            // すべて実施する
            Executions = [
                new ExecutionRequest() {ExamItemId = 1, IsPerforming = true, CancelReasonId = null},
                new ExecutionRequest() {ExamItemId = 2, IsPerforming = true, CancelReasonId = null},
                new ExecutionRequest() {ExamItemId = 71, IsPerforming = true, CancelReasonId = null},
                new ExecutionRequest() {ExamItemId = 72, IsPerforming = true, CancelReasonId = null}
            ]
        };

        // DBの受診情報
        var consult = new Consult()
        {
            ConsultId = Guid.Parse("7400c9cc-c2ee-48ef-a12e-5c3244631a6d"),
            ConsultNumber = "0001",
            Age = new Age(40, 1, 1),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            Note = "定期健康診断",
            ExamineeId = Guid.Parse("9944b7f3-0728-4801-853f-424a9c23a0b9"),
            TicketNumber = "1029"
        };

        // 会場日程ステータス
        var placeScheduleStatus = new PlaceScheduleStatus
        {
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            PlaceId = Guid.Parse("492d6d5c-17ab-4aba-89e2-51369373b8a8"),
            PlaceName = "市役所",
            ExamDate = new DateOnly(2024, 12, 19),
            Status = PlaceScheduleLockingStatus.検査完了,
            CreatedAt = DateTime.Now,
            CreatedBy = "admin"
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleLockingStatusAsync(It.IsAny<Guid>()))
                                    .ReturnsAsync(placeScheduleStatus);
        _staffIdentityProviderMock.Setup(x => x.Role).Returns(Role.User);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act & Assert
        await usecase.Invoking(x => x.RegisterExecutionsAsync(consultNumber, request))
                     .Should().ThrowAsync<PlaceScheduleLockedException>();
    }

    [Fact]
    public async Task 検査結果相関ルールで検証する_リクエスト値なし()
    {
        // Arrange

        var consultNumber = "0002";
        var consultId = Guid.Parse("8dda2a54-5217-425f-bba9-ab821a9647fe");
        var examDate = new DateOnly(2024, 11, 30);

        var consult = new Consult()
        {
            ConsultId = consultId,
            ConsultNumber = consultNumber,
            Age = new Age(0, 0, 0),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            ExamineeId = Guid.Parse("2cd0043d-ff17-4673-a765-86557966f143"),
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            TicketNumber = "1002",
            Note = "定期健康診断"
        };

        var placeSchedule = new WebAPI.ResultCollector.Domain.Models.PlaceSchedule()
        {
            Id = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            Place = new WebAPI.ResultCollector.Domain.Models.Place() { Id = Guid.Parse("45449e64-5632-4fcb-9261-5c136531a86a"), Code = "001", Name = "会場1", OrderNumber = 1 },
            Team = new Team() { Id = Guid.Parse("1eb0a120-a76c-49d7-a2da-47fccf5bef44"), Code = "001", Name = "1班", OrderNumber = 1 },
            ExamDate = examDate,
            StartTime = "1000",
            PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
        };

        // 前回値
        var previousResult = new PreviousResult()
        {
            ConsultId = consultId,
            ExamDate = new DateOnly(2023, 10, 01),
            ExamItemDetailResults = [
                new(){ExamItemId = 1, ExamItemDetailId =5, Value = "70.0"}
            ]
        };

        // DBの今回値
        var examResult = new ExamResult()
        {
            ConsultId = consultId,
            ExamItemDetailResults = [
                new(){ExamItemId = 1, ExamItemDetailId =5, Value = "90.3"}
            ]
        };

        var examMenuId = 5;
        var rules = new List<CorrelationRule>() {
            new(){
                CorrelationRuleId = 1,
                Name = "腹囲_前回差20cm以上",
                ExamMenuId = 5,
                Priority = 1,
                TriggerType = RuleTriggerType.ThresholdExceeded,
                ErrorLevel = InputErrorLevel.警告,
                ExamItemId = 2,
                Message = "腹囲の前回差が20cm以上です。",
                Evaluations = [
                    new CorrelationRuleEvaluation(){VariableNumber = 1, EvaluationValue = "20.0"}
                ],
                ExamItemDetails = [
                    new CorrelationRuleExamItemDetail(){VariableNumber = 1, ExamItemDetailId = 5, SourceType = SourceType.今回値},
                    new CorrelationRuleExamItemDetail(){VariableNumber = 2, ExamItemDetailId = 5, SourceType = SourceType.前回値}
                ]
            }
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"))).ReturnsAsync(placeSchedule);
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId)).ReturnsAsync(examResult);
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate)).ReturnsAsync(previousResult);
        _examItemRepositoryMock.Setup(x => x.GetCorrelationRulesAsync(examMenuId)).ReturnsAsync(rules);

        // リクエスト値はなし
        var resultsRequest = new ResultsRequest()
        {
            ExamMenuId = 5,
            ExamResults = []
        };

        var expected = new List<RuleError>() {
            new(){ErrorLevel = InputErrorLevel.警告, Message = "腹囲の前回差が20cm以上です。", Priority = 1, ExamItemId = 2}
        };

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act
        var errors = await usecase.ValidateCorrelationRuleAsync(consultNumber, resultsRequest);

        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 検査結果相関ルールで検証する_結果値なしトリガー無効()
    {
        // Arrange

        var consultNumber = "0002";
        var consultId = Guid.Parse("8dda2a54-5217-425f-bba9-ab821a9647fe");
        var examDate = new DateOnly(2024, 11, 30);

        var consult = new Consult()
        {
            ConsultId = consultId,
            ConsultNumber = consultNumber,
            Age = new Age(0, 0, 0),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            ExamineeId = Guid.Parse("2cd0043d-ff17-4673-a765-86557966f143"),
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            TicketNumber = "1002",
            Note = "定期健康診断"
        };

        var placeSchedule = new WebAPI.ResultCollector.Domain.Models.PlaceSchedule()
        {
            Id = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            Place = new WebAPI.ResultCollector.Domain.Models.Place() { Id = Guid.Parse("45449e64-5632-4fcb-9261-5c136531a86a"), Code = "001", Name = "会場1", OrderNumber = 1 },
            Team = new Team() { Id = Guid.Parse("1eb0a120-a76c-49d7-a2da-47fccf5bef44"), Code = "001", Name = "1班", OrderNumber = 1 },
            ExamDate = examDate,
            StartTime = "1000",
            PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
        };

        // 前回値
        var previousResult = new PreviousResult()
        {
            ConsultId = consultId,
            ExamDate = new DateOnly(2023, 10, 01),
            ExamItemDetailResults = []
        };

        // DBの今回値
        var examResult = new ExamResult()
        {
            ConsultId = consultId,
            ExamItemDetailResults = []
        };

        var examMenuId = 5;
        var rules = new List<CorrelationRule>() {
            new(){
                CorrelationRuleId = 1,
                Name = "腹囲_前回差20cm以上",
                ExamMenuId = 5,
                Priority = 1,
                TriggerType = RuleTriggerType.ThresholdExceeded,
                ErrorLevel = InputErrorLevel.警告,
                ExamItemId = 2,
                Message = "腹囲の前回差が20cm以上です。",
                Evaluations = [
                    new CorrelationRuleEvaluation(){VariableNumber = 1, EvaluationValue = "20.0"}
                ],
                ExamItemDetails = [
                    new CorrelationRuleExamItemDetail(){VariableNumber = 1, ExamItemDetailId = 5, SourceType = SourceType.今回値},
                    new CorrelationRuleExamItemDetail(){VariableNumber = 2, ExamItemDetailId = 5, SourceType = SourceType.前回値}
                ]
            }
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"))).ReturnsAsync(placeSchedule);
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId)).ReturnsAsync(examResult);
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate)).ReturnsAsync(previousResult);
        _examItemRepositoryMock.Setup(x => x.GetCorrelationRulesAsync(examMenuId)).ReturnsAsync(rules);

        // リクエスト値はなし
        var resultsRequest = new ResultsRequest()
        {
            ExamMenuId = 5,
            ExamResults = []
        };

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act
        var errors = await usecase.ValidateCorrelationRuleAsync(consultNumber, resultsRequest);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public async Task 検査結果相関ルールで検証する_リクエスト値あり()
    {
        // Arrange

        var consultNumber = "0002";
        var consultId = Guid.Parse("8dda2a54-5217-425f-bba9-ab821a9647fe");
        var examDate = new DateOnly(2024, 11, 30);

        var consult = new Consult()
        {
            ConsultId = consultId,
            ConsultNumber = consultNumber,
            Age = new Age(0, 0, 0),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            ExamineeId = Guid.Parse("2cd0043d-ff17-4673-a765-86557966f143"),
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            TicketNumber = "1002",
            Note = "定期健康診断"
        };

        var placeSchedule = new WebAPI.ResultCollector.Domain.Models.PlaceSchedule()
        {
            Id = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            Place = new WebAPI.ResultCollector.Domain.Models.Place() { Id = Guid.Parse("45449e64-5632-4fcb-9261-5c136531a86a"), Code = "001", Name = "会場1", OrderNumber = 1 },
            Team = new Team() { Id = Guid.Parse("1eb0a120-a76c-49d7-a2da-47fccf5bef44"), Code = "001", Name = "1班", OrderNumber = 1 },
            ExamDate = examDate,
            StartTime = "1000",
            PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
        };

        // 前回値
        var previousResult = new PreviousResult()
        {
            ConsultId = consultId,
            ExamDate = new DateOnly(2023, 10, 01),
            ExamItemDetailResults = [
                new(){ExamItemId = 1, ExamItemDetailId =5, Value = "70.0"}
            ]
        };

        // DBの今回値
        var examResult = new ExamResult()
        {
            ConsultId = consultId,
            ExamItemDetailResults = [
                new(){ExamItemId = 1, ExamItemDetailId =5, Value = "80.5"}
            ]
        };

        var examMenuId = 5;
        var rules = new List<CorrelationRule>() {
            new(){
                CorrelationRuleId = 1,
                Name = "腹囲_前回差20cm以上",
                ExamMenuId = 5,
                Priority = 1,
                TriggerType = RuleTriggerType.ThresholdExceeded,
                ErrorLevel = InputErrorLevel.警告,
                ExamItemId = 2,
                Message = "腹囲の前回差が20cm以上です。",
                Evaluations = [
                    new CorrelationRuleEvaluation(){VariableNumber = 1, EvaluationValue = "20.0"}
                ],
                ExamItemDetails = [
                    new CorrelationRuleExamItemDetail(){VariableNumber = 1, ExamItemDetailId = 5, SourceType = SourceType.今回値},
                    new CorrelationRuleExamItemDetail(){VariableNumber = 2, ExamItemDetailId = 5, SourceType = SourceType.前回値}
                ]
            },
            new(){
                CorrelationRuleId = 2,
                Name = "リクエスト値が同じであること",
                ExamMenuId = 5,
                Priority = 1,
                TriggerType = RuleTriggerType.AllInputsNotEqual,
                ErrorLevel = InputErrorLevel.異常,
                ExamItemId = 77,
                Message = "登録する値が異なります。",
                Evaluations = [],
                ExamItemDetails = [
                    new CorrelationRuleExamItemDetail(){VariableNumber = 1, ExamItemDetailId = 771, SourceType = SourceType.今回値},
                    new CorrelationRuleExamItemDetail(){VariableNumber = 2, ExamItemDetailId = 772, SourceType = SourceType.今回値},
                    new CorrelationRuleExamItemDetail(){VariableNumber = 3, ExamItemDetailId = 773, SourceType = SourceType.今回値},
                    new CorrelationRuleExamItemDetail(){VariableNumber = 4, ExamItemDetailId = 774, SourceType = SourceType.今回値},
                    new CorrelationRuleExamItemDetail(){VariableNumber = 5, ExamItemDetailId = 775, SourceType = SourceType.今回値}
                ]
            }
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"))).ReturnsAsync(placeSchedule);
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId)).ReturnsAsync(examResult);
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate)).ReturnsAsync(previousResult);
        _examItemRepositoryMock.Setup(x => x.GetCorrelationRulesAsync(examMenuId)).ReturnsAsync(rules);

        // リクエスト値による上書きあり
        var resultsRequest = new ResultsRequest()
        {
            ExamMenuId = 5,
            ExamResults = [
                new  ResultRequest(){
                    ExamItemId = 2,
                    ExamItemDetails = [
                        new ExamItemDetailRequest(){ExamItemDetailId = 5, Value = "93.8"}
                    ]
                },
                new  ResultRequest(){
                    ExamItemId = 77,
                    ExamItemDetails = [
                        new ExamItemDetailRequest(){ExamItemDetailId = 771, Value = "ABC1001"},
                        new ExamItemDetailRequest(){ExamItemDetailId = 772, Value = "ABC1001"},
                        new ExamItemDetailRequest(){ExamItemDetailId = 773, Value = "ABC1001"},
                        new ExamItemDetailRequest(){ExamItemDetailId = 774, Value = "ABC1001"},
                        new ExamItemDetailRequest(){ExamItemDetailId = 775, Value = ""}
                    ]
                }
            ]
        };

        var expected = new List<RuleError>() {
            new(){ErrorLevel = InputErrorLevel.警告, Message = "腹囲の前回差が20cm以上です。", Priority = 1, ExamItemId = 2},
            new(){ErrorLevel = InputErrorLevel.異常, Message = "登録する値が異なります。", Priority = 1, ExamItemId = 77}
        };

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act
        var errors = await usecase.ValidateCorrelationRuleAsync(consultNumber, resultsRequest);

        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 検査実施判断ルールで検証する_対象検査あり()
    {
        // Arrange

        var consultNumber = "0006";
        var consultId = Guid.Parse("8dda2a54-5217-425f-bba9-ab821a9647fe");
        var examDate = new DateOnly(2024, 11, 30);

        var consult = new Consult()
        {
            ConsultId = consultId,
            ConsultNumber = consultNumber,
            Age = new Age(0, 0, 0),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            ExamineeId = Guid.Parse("2cd0043d-ff17-4673-a765-86557966f143"),
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            TicketNumber = "1002",
            Note = "定期健康診断"
        };

        var placeSchedule = new WebAPI.ResultCollector.Domain.Models.PlaceSchedule()
        {
            Id = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            Place = new WebAPI.ResultCollector.Domain.Models.Place() { Id = Guid.Parse("45449e64-5632-4fcb-9261-5c136531a86a"), Code = "001", Name = "会場1", OrderNumber = 1 },
            Team = new Team() { Id = Guid.Parse("1eb0a120-a76c-49d7-a2da-47fccf5bef44"), Code = "001", Name = "1班", OrderNumber = 1 },
            ExamDate = examDate,
            StartTime = "1000",
            PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
        };

        // 前回値
        var previousResult = new PreviousResult()
        {
            ConsultId = consultId,
            ExamDate = new DateOnly(2023, 10, 01),
            ExamItemDetailResults = [
                new(){ExamItemId = 1, ExamItemDetailId =5, Value = "70.0"}
            ]
        };

        // DBの今回値
        var examResult = new ExamResult()
        {
            ConsultId = consultId,
            ExamItemDetailResults = [
                new(){ExamItemId = 1, ExamItemDetailId =5, Value = "105.5"}
            ]
        };

        var examMenuId = 7;
        var rules = new List<DecisionRule>() {
            new(){
                DecisionRuleId = 1,
                Name = "体重差が前年より20kgオーバー",
                ExamMenuId = 7,
                Priority = 1,
                TriggerType = RuleTriggerType.ThresholdExceeded,
                ErrorLevel = InputErrorLevel.異常,
                Message = "体重の計測ミスのため実施できません。",
                Evaluations = [
                    new DecisionRuleEvaluation(){VariableNumber = 1, EvaluationValue = "30.0"}
                ],
                ExamItemDetails = [
                    new DecisionRuleExamItemDetail(){VariableNumber = 1, ExamItemDetailId = 5, SourceType = SourceType.今回値},
                    new DecisionRuleExamItemDetail(){VariableNumber = 2, ExamItemDetailId = 5, SourceType = SourceType.前回値}
                ]
            }
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"))).ReturnsAsync(placeSchedule);
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId)).ReturnsAsync(examResult);
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate)).ReturnsAsync(previousResult);
        _examItemRepositoryMock.Setup(x => x.GetDecisionRulesAsync(examMenuId)).ReturnsAsync(rules);

        var expected = new List<RuleError>() {
            new(){ErrorLevel = InputErrorLevel.異常, Message = "体重の計測ミスのため実施できません。", Priority = 1}
        };

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act
        var errors = await usecase.ValidateDecisionRuleAsync(examMenuId, examResult, previousResult);

        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 検査実施判断ルールで検証する_対象検査なし()
    {
        // Arrange

        var consultNumber = "0006";
        var consultId = Guid.Parse("8dda2a54-5217-425f-bba9-ab821a9647fe");
        var examDate = new DateOnly(2024, 11, 30);

        var consult = new Consult()
        {
            ConsultId = consultId,
            ConsultNumber = consultNumber,
            Age = new Age(0, 0, 0),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            ExamineeId = Guid.Parse("2cd0043d-ff17-4673-a765-86557966f143"),
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            TicketNumber = "1002",
            Note = "定期健康診断"
        };

        var placeSchedule = new WebAPI.ResultCollector.Domain.Models.PlaceSchedule()
        {
            Id = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            Place = new WebAPI.ResultCollector.Domain.Models.Place() { Id = Guid.Parse("45449e64-5632-4fcb-9261-5c136531a86a"), Code = "001", Name = "会場1", OrderNumber = 1 },
            Team = new Team() { Id = Guid.Parse("1eb0a120-a76c-49d7-a2da-47fccf5bef44"), Code = "001", Name = "1班", OrderNumber = 1 },
            ExamDate = examDate,
            StartTime = "1000",
            PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
        };

        // 前回値
        var previousResult = new PreviousResult()
        {
            ConsultId = consultId,
            ExamDate = new DateOnly(2023, 10, 01),
            ExamItemDetailResults = [
                new(){ExamItemId = 1, ExamItemDetailId =5, Value = "70.0"}
            ]
        };

        // DBの今回値
        var examResult = new ExamResult()
        {
            ConsultId = consultId,
            ExamItemDetailResults = [
                new(){ExamItemId = 1, ExamItemDetailId =5, Value = "75.5"}
            ]
        };

        var examMenuId = 7;
        var rules = new List<DecisionRule>() {
            new(){
                DecisionRuleId = 1,
                Name = "体重差が前年より20kgオーバー",
                ExamMenuId = 7,
                Priority = 1,
                TriggerType = RuleTriggerType.ThresholdExceeded,
                ErrorLevel = InputErrorLevel.異常,
                Message = "体重の計測ミスのため実施できません。",
                Evaluations = [
                    new DecisionRuleEvaluation(){VariableNumber = 1, EvaluationValue = "30.0"}
                ],
                ExamItemDetails = [
                    new DecisionRuleExamItemDetail(){VariableNumber = 1, ExamItemDetailId = 5, SourceType = SourceType.今回値},
                    new DecisionRuleExamItemDetail(){VariableNumber = 2, ExamItemDetailId = 5, SourceType = SourceType.前回値}
                ]
            }
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"))).ReturnsAsync(placeSchedule);
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId)).ReturnsAsync(examResult);
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate)).ReturnsAsync(previousResult);
        _examItemRepositoryMock.Setup(x => x.GetDecisionRulesAsync(examMenuId)).ReturnsAsync(rules);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act
        var errors = await usecase.ValidateDecisionRuleAsync(examMenuId, examResult, previousResult);

        // Assert
        errors.Should().NotBeNull();
    }

    [Fact]
    public async Task 検査実施判断ルールで検証する_結果値なしトリガー無効()
    {
        // Arrange

        var consultNumber = "0006";
        var consultId = Guid.Parse("8dda2a54-5217-425f-bba9-ab821a9647fe");
        var examDate = new DateOnly(2024, 11, 30);

        var consult = new Consult()
        {
            ConsultId = consultId,
            ConsultNumber = consultNumber,
            Age = new Age(0, 0, 0),
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            ExamineeId = Guid.Parse("2cd0043d-ff17-4673-a765-86557966f143"),
            PlaceScheduleId = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            TicketNumber = "1002",
            Note = "定期健康診断"
        };

        var placeSchedule = new WebAPI.ResultCollector.Domain.Models.PlaceSchedule()
        {
            Id = Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"),
            Place = new WebAPI.ResultCollector.Domain.Models.Place() { Id = Guid.Parse("45449e64-5632-4fcb-9261-5c136531a86a"), Code = "001", Name = "会場1", OrderNumber = 1 },
            Team = new Team() { Id = Guid.Parse("1eb0a120-a76c-49d7-a2da-47fccf5bef44"), Code = "001", Name = "1班", OrderNumber = 1 },
            ExamDate = examDate,
            StartTime = "1000",
            PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
        };

        // 前回値
        var previousResult = new PreviousResult()
        {
            ConsultId = consultId,
            ExamDate = new DateOnly(2023, 10, 01),
            ExamItemDetailResults = []
        };

        // DBの今回値
        var examResult = new ExamResult()
        {
            ConsultId = consultId,
            ExamItemDetailResults = [
                new(){ExamItemId = 1, ExamItemDetailId =5, Value = "105.5"}
            ]
        };

        var examMenuId = 7;
        var rules = new List<DecisionRule>() {
            new(){
                DecisionRuleId = 1,
                Name = "体重差が前年より20kgオーバー",
                ExamMenuId = 7,
                Priority = 1,
                TriggerType = RuleTriggerType.ThresholdExceeded,
                ErrorLevel = InputErrorLevel.異常,
                Message = "体重の計測ミスのため実施できません。",
                Evaluations = [
                    new DecisionRuleEvaluation(){VariableNumber = 1, EvaluationValue = "30.0"}
                ],
                ExamItemDetails = [
                    new DecisionRuleExamItemDetail(){VariableNumber = 1, ExamItemDetailId = 5, SourceType = SourceType.今回値},
                    new DecisionRuleExamItemDetail(){VariableNumber = 2, ExamItemDetailId = 5, SourceType = SourceType.前回値}
                ]
            }
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(Guid.Parse("8cdd7c4a-e196-438e-a02e-54cb1af932c1"))).ReturnsAsync(placeSchedule);
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId)).ReturnsAsync(examResult);
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate)).ReturnsAsync(previousResult);
        _examItemRepositoryMock.Setup(x => x.GetDecisionRulesAsync(examMenuId)).ReturnsAsync(rules);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);

        // Act
        var errors = await usecase.ValidateDecisionRuleAsync(examMenuId, examResult, previousResult);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public async Task 検査結果を登録する()
    {
        // Arrange
        var consultNumber = "0006";
        var consultId = Guid.Parse("8d670eb8-d9f2-40b2-bfa6-cef6403be53d");
        var examDate = new DateOnly(2025, 1, 10);

        // 会場日程
        var placeSchedule = new WebAPI.ResultCollector.Domain.Models.PlaceSchedule()
        {
            Id = Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250"),
            Place = new WebAPI.ResultCollector.Domain.Models.Place() { Id = Guid.Parse("45449e64-5632-4fcb-9261-5c136531a86a"), Code = "001", Name = "会場1", OrderNumber = 1 },
            Team = new Team() { Id = Guid.Parse("1eb0a120-a76c-49d7-a2da-47fccf5bef44"), Code = "001", Name = "1班", OrderNumber = 1 },
            ExamDate = examDate,
            StartTime = "1000",
            PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
        };

        // 受診者
        var examinee = new WebAPI.ResultCollector.Domain.Models.Examinee()
        {
            ExamineeId = Guid.Parse("85417626-3b52-44f1-82cc-2bad8e43d5df"),
            ExamineeCode = "10001",
            Name = "両備　太郎",
            KanaName = "リョウビ　タロウ",
            Sex = Sex.男,
            Birthdate = new Birthdate(new DateOnly(1999, 11, 29)),
            Affiliations = [new Affiliations { OrganizationId = Guid.Parse("7bf76a3d-8bb4-41f5-8bd5-3b8fc8482511"), OrganizationCode = "0001", OrganizationName = "", OrderNumber = 1 }]
        };

        // 受診
        var consult = new WebAPI.ResultCollector.Domain.Models.Consult()
        {
            ConsultId = consultId,
            ConsultNumber = consultNumber,
            Age = new Age(0, 0, 0),
            ExamineeId = Guid.Parse("85417626-3b52-44f1-82cc-2bad8e43d5df"),
            ProgressStatus = ConsultProgressStatus.検査中,
            Note = "定期健康診断",
            ExportStatus = ConsultResultExportStatus.未出力,
            PlaceScheduleId = Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250"),
            TicketNumber = "1029"
        };
        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);

        // DBの今回値
        var examResult = new ExamResult()
        {
            ConsultId = consultId,
            ExamItemDetailResults = []
        };

        // DBの前回値
        var previousResult = new PreviousResult()
        {
            ConsultId = consultId,
            ExamDate = new DateOnly(2024, 4, 1),
            ExamItemDetailResults = []
        };



        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId)).ReturnsAsync(examResult);
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate)).ReturnsAsync(previousResult);
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250"))).ReturnsAsync(placeSchedule);
        _examineeRepositoryMock.Setup(x => x.GetExamineeAsync(Guid.Parse("85417626-3b52-44f1-82cc-2bad8e43d5df"))).ReturnsAsync(examinee);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);
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
                     .Should().NotThrowAsync();
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
                                Age = new Age(0, 0, 0),
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
                                        ExamDate = new DateOnly(2024, 12, 19),
                                        Status = PlaceScheduleLockingStatus.検査完了,
                                        CreatedAt = DateTime.Now,
                                        CreatedBy = "admin"
                                    });
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);
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

    [Fact]
    public async Task 検査内容を取得する()
    {
        // Arrange
        var consultNumber = "0006";
        var examMenuId = 7;
        var examineeCode = "100020";
        var consultId = Guid.Parse("8d670eb8-d9f2-40b2-bfa6-cef6403be53d");
        var examineeId = Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3");
        var placeScheduleId = Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250");
        var organizationId = Guid.Parse("7bf76a3d-8bb4-41f5-8bd5-3b8fc8482511");
        var examDate = new DateOnly(2024, 11, 30);
        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber))
                            .ReturnsAsync(new Consult
                            {
                                ConsultId = consultId,
                                ConsultNumber = consultNumber,
                                Age = new Age(58, 8, 5),
                                ExamineeId = examineeId,
                                ProgressStatus = ConsultProgressStatus.検査中,
                                Note = "定期健康診断",
                                ExportStatus = ConsultResultExportStatus.未出力,
                                PlaceScheduleId = placeScheduleId,
                                TicketNumber = "8931"
                            });
        _examineeRepositoryMock.Setup(x => x.GetExamineeAsync(examineeId))
                            .ReturnsAsync(new Examinee
                            {
                                ExamineeId = examineeId,
                                ExamineeCode = examineeCode,
                                Name = "両備　太郎",
                                KanaName = "リョウビ　タロウ",
                                Sex = Sex.男,
                                Birthdate = new Birthdate(new DateOnly(1966, 3, 25)),
                                Affiliations = [new Affiliations { OrganizationId = organizationId, OrganizationCode = "0001",
                                                                   OrganizationName = "両備システムズ", OrderNumber = 1 }]
                            });
        // 会場日程IDを指定して会場日程を取得する
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(placeScheduleId))
                            .ReturnsAsync(new PlaceSchedule
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
                                StartTime = "1000",
                                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
                            });
        // 検査メニューに関連した検査項目情報を取得
        _examItemRepositoryMock.Setup(x => x.GetExamItemGroupsAsync(examMenuId))
                            .ReturnsAsync([
                                new ExamItemGroup{ ExamItemGroupId = 7, Type = ExamItemGroupType.数値,
                                                   ExamItems = [
                                                        new ExamItem{ PositionNumber = 1,  ExamItemId = 71, Name = "血圧1",
                                                                      ExamItemDetails = [
                                                                            new ExamItemDetail{ PositionNumber = 1, ExamItemDetailId = 711, EquipmentLabel = null,
                                                                                                Name = "血圧1_上", Unit = null, Type = ExamItemDetailType.入力,
                                                                                                IntegerLength = 3, DecimalLength = 0, KeyboardType = KeyboardType.テンキー},
                                                                            new ExamItemDetail{ PositionNumber = 2, ExamItemDetailId = 712, EquipmentLabel = null,
                                                                                                Name = "血圧1_下", Unit = null, Type = ExamItemDetailType.入力,
                                                                                                IntegerLength = 3, DecimalLength = 0, KeyboardType = KeyboardType.テンキー}
                                                                      ] },
                                                        new ExamItem{ PositionNumber = 2,  ExamItemId = 72, Name = "血圧2",
                                                                      ExamItemDetails = [
                                                                            new ExamItemDetail{ PositionNumber = 1, ExamItemDetailId = 721, EquipmentLabel = null,
                                                                                                Name = "血圧2_上", Unit = null, Type = ExamItemDetailType.入力,
                                                                                                IntegerLength = 3, DecimalLength = 0, KeyboardType = KeyboardType.テンキー},
                                                                            new ExamItemDetail{ PositionNumber = 2, ExamItemDetailId = 722, EquipmentLabel = null,
                                                                                                Name = "血圧2_下", Unit = null, Type = ExamItemDetailType.入力,
                                                                                                IntegerLength = 3, DecimalLength = 0, KeyboardType = KeyboardType.テンキー }
                                                                      ] }
                                ] } ]);
        // 検査中止を取得
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consultId))
                                .ReturnsAsync(new ExamCancel
                                {
                                    ConsultId = consultId,
                                    ExamItemDetailCancels = [
                                        new ExamItemDetailCancel{ExamItemId=72, ExamItemDetailId =721 ,CancelReasonId= 10 },
                                        new ExamItemDetailCancel{ExamItemId=72, ExamItemDetailId =722 ,CancelReasonId= 10 }
                                    ]
                                });
        // 検査依頼を取得
        _consultRepositoryMock.Setup(x => x.GetExamOrdersAsync(consultId))
                                .ReturnsAsync(new ExamOrder
                                {
                                    ConsultId = consultId,
                                    ExamItemDetailOrders = [
                                        new ExamItemDetailOrder{ ExamItemId=71, ExamItemDetailId =711 },
                                        new ExamItemDetailOrder{ ExamItemId=71, ExamItemDetailId =712 }
                                    ]
                                });
        // 未受診の検査メニューを取得
        _consultRepositoryMock.Setup(x => x.GetUnexaminedConsultsAsync(new string[] { consultNumber }))
                                .ReturnsAsync([
                                    new UnexaminedConsult {
                                        ConsultId = consultId, ConsultNumber = consultNumber, ExamineeId = examineeId,
                                        UnexaminedExamMenus = [
                                            new UnexaminedExamMenu { ExamMenuId = 1, ExamMenuName = "身体計測" },
                                            new UnexaminedExamMenu { ExamMenuId = 7, ExamMenuName = "血圧"}
                                        ] } ]);
        // 同姓同名アラート
        _placeScheduleRepositoryMock.Setup(x => x.IsSamenameAsync(consultNumber)).ReturnsAsync(false);
        // 受診特記一覧を取得
        _consultRepositoryMock.Setup(x => x.GetConsultNotesAsync(consultId))
                                .ReturnsAsync([
                                    new ConsultNote { Code = "ST01", Note = "01-001" },
                                    new ConsultNote { Code = "ST02", Note = "02-001" }]);
        // 過去検査結果を取得
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate))
                                .ReturnsAsync(new PreviousResult
                                {
                                    ConsultId = consultId,
                                    ExamDate = new DateOnly(2023, 04, 10),
                                    ExamItemDetailResults = [
                                        new ExamItemDetailResult{ ExamItemId = 2, ExamItemDetailId = 2, Value  = "70.0" },
                                        new ExamItemDetailResult{ ExamItemId = 23, ExamItemDetailId = 230, Value  = "20" }
                                    ]
                                });
        // 検査メニュー特記一覧を取得
        _examMenuRepositoryMock.Setup(x => x.GetMenuNotesAsync(examMenuId))
                                .ReturnsAsync([
                                    new MenuNote{ MenuNoteId = 2, Name = "撮影番号/○○番号", ExamMenuId = 7, Suffix = "番",
                                                  ConsultNotes = [
                                                        new MenuNoteConsult{ Code = "ST01" },
                                                        new MenuNoteConsult{ Code = "ST02" }
                                                  ],
                                        ExamResults = []
                                    },
                                    new MenuNote{ MenuNoteId = 2, Name = "検査メニュー特記用", ExamMenuId = 2, Suffix = "",
                                                  ConsultNotes = [],
                                                  ExamResults = [
                                                    new  MenuNoteExamResult{
                                                        ExamItemDetailId = 230,
                                                        SourceType = SourceType.今回値
                                                    },
                                                    new  MenuNoteExamResult{
                                                        ExamItemDetailId = 230,
                                                        SourceType = SourceType.前回値
                                                    }
                                                  ]
                                    }
                                ]);
        // 検査結果を取得
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId))
                                .ReturnsAsync(new ExamResult
                                {
                                    ConsultId = consultId,
                                    ExamItemDetailResults = [
                                        new ExamItemDetailResult{ ExamItemId = 2, ExamItemDetailId = 2, Value = "160.5" },
                                        new ExamItemDetailResult{ ExamItemId = 23, ExamItemDetailId = 230, Value = "30" }
                                    ]
                                });

        // 検査項目明細マスタ一覧を取得
        _examItemRepositoryMock.Setup(x => x.GetExamItemDetailChildrenAsync(new int[] { 230 })).ReturnsAsync([
            new ExamItemDetailChild{
                ExamItemDetailId = 230,
                Name = "検査メニュー特記用明細",
                PositionNumber = 0,
                EquipmentLabel = "",
                Unit = "",
                Type = ExamItemDetailType.入力,
                IntegerLength = 3,
                DecimalLength = 0,
                KeyboardType = KeyboardType.テンキー,
                Keyboards = [],
                DetailOptions = []
            }
        ]);

        // 検査実施判断ルールで検証する
        _examItemRepositoryMock.Setup(x => x.GetDecisionRulesAsync(examMenuId))
                                .ReturnsAsync([
                                    new DecisionRule{ DecisionRuleId = 1, Name = "体重差が前年より30kgオーバー", ExamMenuId = examMenuId,
                                                      Priority = 1, TriggerType = RuleTriggerType.ThresholdExceeded, ErrorLevel = InputErrorLevel.異常,
                                                      Message = "体重の計測ミスのため実施できません。",
                                                      Evaluations = [new DecisionRuleEvaluation(){VariableNumber = 1, EvaluationValue = "30.0"}],
                                                      ExamItemDetails = [
                                                         new DecisionRuleExamItemDetail{ VariableNumber = 1, SourceType = SourceType.今回値, ExamItemDetailId = 2 },
                                                         new DecisionRuleExamItemDetail{ VariableNumber = 2, SourceType = SourceType.前回値, ExamItemDetailId = 2 }
                                                      ]
                                       }
                                ]);

        var examContent = new APIModels.Responses.ExamContent
        {
            ConsultNumber = "0006",
            ConsultName = "定期健康診断",
            Examinee =
                new APIModels.Responses.Examinee
                {
                    TicketNumber = "8931",
                    Name = "両備　太郎",
                    KanaName = "リョウビ　タロウ",
                    Birthdate = DateOnly.Parse("1966-03-25"),
                    Sex = (int)Sex.男,
                    Organizations = ["両備システムズ"],
                    SameNameAlert = false,
                    ExamDateAge = 58
                },
            IsComplete = false,
            RelatedExamItems = [
                new APIModels.Responses.RelatedExamItem{ ExamItemName = "撮影番号/○○番号", ExamResult = "01-001/02-001番" },
                new APIModels.Responses.RelatedExamItem{ ExamItemName = "検査メニュー特記用", ExamResult = "30(20)" }
            ],
            ExamItems = [
                new APIModels.Responses.ExamDetail{ ExamItemId = 71, ExamItemName = "血圧1", HasOrder = true, CancelReasonId = null},
                new APIModels.Responses.ExamDetail{ ExamItemId = 72, ExamItemName = "血圧2", HasOrder = false, CancelReasonId = 10}
            ],
            ExamDecisionResults = [
                new APIModels.Responses.ExamDecisionResult{ ErrorLevel = (int)InputErrorLevel.異常, Description = "体重の計測ミスのため実施できません。" }
            ],
            UnexaminedItems = [
                new APIModels.Responses.ExamMenu{ ExamMenuId = 1, ExamMenuName = "身体計測"},
                new APIModels.Responses.ExamMenu{ ExamMenuId = 7, ExamMenuName = "血圧"}
            ]
        };

        // Act
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);
        var response = await usecase.GetExamItemsExamineeAsync(consultNumber, examMenuId);

        // Assert
        response.Should().BeEquivalentTo(examContent);

    }

    [Fact]
    public async Task 検査内容を取得する_IsCompleteがtrue()
    {
        // Arrange
        var consultNumber = "0006";
        var examMenuId = 7;
        var examineeCode = "100020";
        var consultId = Guid.Parse("8d670eb8-d9f2-40b2-bfa6-cef6403be53d");
        var examineeId = Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3");
        var placeScheduleId = Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250");
        var organizationId = Guid.Parse("7bf76a3d-8bb4-41f5-8bd5-3b8fc8482511");
        var examDate = new DateOnly(2024, 11, 30);
        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber))
                            .ReturnsAsync(new Consult
                            {
                                ConsultId = consultId,
                                ConsultNumber = consultNumber,
                                Age = new Age(58, 8, 5),
                                ExamineeId = examineeId,
                                ProgressStatus = ConsultProgressStatus.検査中,
                                Note = "定期健康診断",
                                ExportStatus = ConsultResultExportStatus.未出力,
                                PlaceScheduleId = placeScheduleId,
                                TicketNumber = "8931"
                            });
        _examineeRepositoryMock.Setup(x => x.GetExamineeAsync(examineeId))
                            .ReturnsAsync(new Examinee
                            {
                                ExamineeId = examineeId,
                                ExamineeCode = examineeCode,
                                Name = "両備　太郎",
                                KanaName = "リョウビ　タロウ",
                                Sex = Sex.男,
                                Birthdate = new Birthdate(new DateOnly(1966, 3, 25)),
                                Affiliations = [new Affiliations { OrganizationId = organizationId, OrganizationCode = "0001",
                                                                   OrganizationName = "両備システムズ", OrderNumber = 1 }]
                            });
        // 会場日程IDを指定して会場日程を取得する
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(placeScheduleId))
                            .ReturnsAsync(new PlaceSchedule
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
                                StartTime = "1000",
                                PlaceScheduleLockingStatus = PlaceScheduleLockingStatus.検査中
                            });
        // 検査メニューに関連した検査項目情報を取得
        _examItemRepositoryMock.Setup(x => x.GetExamItemGroupsAsync(examMenuId))
                               .ReturnsAsync([
                                new ExamItemGroup{ ExamItemGroupId = 7, Type = ExamItemGroupType.数値,
                                                   ExamItems = [
                                                        new ExamItem{ PositionNumber = 1,  ExamItemId = 71, Name = "血圧1",
                                                                      ExamItemDetails = [
                                                                            new ExamItemDetail{ PositionNumber = 1, ExamItemDetailId = 711, EquipmentLabel = "",
                                                                                                Name = "血圧1_上", Unit = "", Type = ExamItemDetailType.入力,
                                                                                                IntegerLength = 3, DecimalLength = 0, KeyboardType = KeyboardType.テンキー},
                                                                            new ExamItemDetail{ PositionNumber = 2, ExamItemDetailId = 712, EquipmentLabel = "",
                                                                                                Name = "血圧1_下", Unit = "", Type = ExamItemDetailType.入力,
                                                                                                IntegerLength = 3, DecimalLength = 0, KeyboardType = KeyboardType.テンキー}
                                                                      ] },
                                                        new ExamItem{ PositionNumber = 2,  ExamItemId = 72, Name = "血圧2",
                                                                      ExamItemDetails = [
                                                                            new ExamItemDetail{ PositionNumber = 1, ExamItemDetailId = 721, EquipmentLabel = "",
                                                                                                Name = "血圧2_上", Unit = "", Type = ExamItemDetailType.入力,
                                                                                                IntegerLength = 3, DecimalLength = 0, KeyboardType = KeyboardType.テンキー},
                                                                            new ExamItemDetail{ PositionNumber = 2, ExamItemDetailId = 722, EquipmentLabel = "",
                                                                                                Name = "血圧2_下", Unit = "", Type = ExamItemDetailType.入力,
                                                                                                IntegerLength = 3, DecimalLength = 0, KeyboardType = KeyboardType.テンキー }
                                                                      ] }
                                ]}]);
        // 検査中止を取得
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consultId))
                                .ReturnsAsync(new ExamCancel
                                {
                                    ConsultId = consultId,
                                    ExamItemDetailCancels = [
                                        new ExamItemDetailCancel{ExamItemId=72, ExamItemDetailId =721 ,CancelReasonId= 10 },
                                        new ExamItemDetailCancel{ExamItemId=72, ExamItemDetailId =722 ,CancelReasonId= 10 }
                                    ]
                                });
        // 検査依頼を取得
        _consultRepositoryMock.Setup(x => x.GetExamOrdersAsync(consultId))
                                .ReturnsAsync(new ExamOrder
                                {
                                    ConsultId = consultId,
                                    ExamItemDetailOrders = [
                                        new ExamItemDetailOrder{ ExamItemId=71, ExamItemDetailId =711 },
                                        new ExamItemDetailOrder{ ExamItemId=71, ExamItemDetailId =712 }
                                    ]
                                });
        // 未受診の検査メニューを取得
        _consultRepositoryMock.Setup(x => x.GetUnexaminedConsultsAsync(new string[] { consultNumber }))
                              .ReturnsAsync([new UnexaminedConsult {
                                ConsultId = consultId, ConsultNumber = consultNumber, ExamineeId = examineeId,
                                UnexaminedExamMenus = [new UnexaminedExamMenu { ExamMenuId = 1, ExamMenuName = "身体計測"}]
                              }]);

        // 同姓同名アラート
        _placeScheduleRepositoryMock.Setup(x => x.IsSamenameAsync(consultNumber)).ReturnsAsync(false);

        // 受診特記一覧を取得
        _consultRepositoryMock.Setup(x => x.GetConsultNotesAsync(consultId)).ReturnsAsync([]);

        // 過去検査結果を取得
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate))
                                .ReturnsAsync(new PreviousResult
                                {
                                    ConsultId = consultId,
                                    ExamDate = new DateOnly(2023, 04, 10),
                                    ExamItemDetailResults = [

                                    ]
                                });
        // 検査メニュー特記一覧を取得
        _examMenuRepositoryMock.Setup(x => x.GetMenuNotesAsync(examMenuId)).ReturnsAsync([]);

        // 検査結果を取得
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId))
                              .ReturnsAsync(new ExamResult
                              {
                                  ConsultId = consultId,
                                  ExamItemDetailResults = [new ExamItemDetailResult { ExamItemId = 2, ExamItemDetailId = 2, Value = "160.5" }]
                              });

        // 検査項目明細マスタ一覧を取得
        _examItemRepositoryMock.Setup(x => x.GetExamItemDetailChildrenAsync(new int[] { 230 })).ReturnsAsync([
            new ExamItemDetailChild{
                ExamItemDetailId = 230,
                Name = "検査メニュー特記用明細",
                PositionNumber = 0,
                EquipmentLabel = "",
                Unit = "",
                Type = ExamItemDetailType.入力,
                IntegerLength = 3,
                DecimalLength = 0,
                KeyboardType = KeyboardType.テンキー,
                Keyboards = [],
                DetailOptions = []
            }
        ]);

        // 検査実施判断ルールで検証する
        _examItemRepositoryMock.Setup(x => x.GetDecisionRulesAsync(examMenuId))
                                .ReturnsAsync([]);

        var expected = new APIModels.Responses.ExamContent
        {
            ConsultNumber = "0006",
            ConsultName = "定期健康診断",
            Examinee =
                new APIModels.Responses.Examinee
                {
                    TicketNumber = "8931",
                    Name = "両備　太郎",
                    KanaName = "リョウビ　タロウ",
                    Birthdate = DateOnly.Parse("1966-03-25"),
                    Sex = (int)Sex.男,
                    Organizations = ["両備システムズ"],
                    SameNameAlert = false,
                    ExamDateAge = 58
                },
            IsComplete = true,
            RelatedExamItems = [],
            ExamItems = [
                new APIModels.Responses.ExamDetail{ ExamItemId = 71, ExamItemName = "血圧1", HasOrder = true, CancelReasonId = null},
                new APIModels.Responses.ExamDetail{ ExamItemId = 72, ExamItemName = "血圧2", HasOrder = false, CancelReasonId = 10}
            ],
            ExamDecisionResults = [],
            UnexaminedItems = [
                new APIModels.Responses.ExamMenu{ ExamMenuId = 1, ExamMenuName = "身体計測"}
            ]
        };

        // Act
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);
        var response = await usecase.GetExamItemsExamineeAsync(consultNumber, examMenuId);

        // Assert
        response.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 検査結果入力情報を取得する()
    {
        // Arrange
        var consultNumber = "0001";
        var examMenuId = 1;
        var examineeCode = "100002";
        var consultId = Guid.Parse("8d670eb8-d9f2-40b2-bfa6-cef6403be53d");
        var examineeId = Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3");
        var placeScheduleId = Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250");
        var organizationId = Guid.Parse("7bf76a3d-8bb4-41f5-8bd5-3b8fc8482511");
        var examDate = new DateOnly(2024, 10, 02);
        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber))
                            .ReturnsAsync(new Consult
                            {
                                ConsultId = consultId,
                                ConsultNumber = consultNumber,
                                Age = new Age(48, 3, 7),
                                ExamineeId = examineeId,
                                ProgressStatus = ConsultProgressStatus.来場待ち,
                                Note = "定期健康診断",
                                ExportStatus = ConsultResultExportStatus.未出力,
                                PlaceScheduleId = placeScheduleId,
                                TicketNumber = "5963"
                            });
        _examineeRepositoryMock.Setup(x => x.GetExamineeAsync(examineeId))
                            .ReturnsAsync(new Examinee
                            {
                                ExamineeId = examineeId,
                                ExamineeCode = examineeCode,
                                Name = "両備　花子",
                                KanaName = "リョウビ　ハナコ",
                                Sex = Sex.女,
                                Birthdate = new Birthdate(new DateOnly(1976, 6, 25)),
                                Affiliations = [new Affiliations { OrganizationId = organizationId, OrganizationCode = "0001",
                                                                   OrganizationName = "両備システムズ", OrderNumber = 1 }]
                            });
        // 会場日程IDを指定して会場日程を取得する
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(placeScheduleId))
                            .ReturnsAsync(new PlaceSchedule
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
                            });
        // 検査メニューに関連した検査項目情報を取得
        _examItemRepositoryMock.Setup(x => x.GetExamItemGroupsAsync(examMenuId))
                            .ReturnsAsync([ new ExamItemGroup{ ExamItemGroupId = 1, Type = ExamItemGroupType.数値,
                                                   ExamItems = [
                                                        new ExamItem { PositionNumber = 1,  ExamItemId = 1, Name = "身長",
                                                                       ExamItemDetails = [ new ExamItemDetail {
                                                                                PositionNumber = 1,
                                                                                ExamItemDetailId =1,
                                                                                EquipmentLabel = "height",
                                                                                Name = "身長",
                                                                                Unit = "cm",
                                                                                Type = ExamItemDetailType.入力,
                                                                                IntegerLength = 3,
                                                                                DecimalLength = 2,
                                                                                KeyboardType = KeyboardType.テンキー
                                                                      }]
                                                        },
                                                        new ExamItem { PositionNumber = 2,  ExamItemId = 2, Name = "体重",
                                                                       ExamItemDetails = [ new ExamItemDetail {
                                                                                PositionNumber = 2,
                                                                                ExamItemDetailId =2,
                                                                                EquipmentLabel = "weight",
                                                                                Name = "体重",
                                                                                Unit = "kg",
                                                                                Type = ExamItemDetailType.入力,
                                                                                IntegerLength = 3,
                                                                                DecimalLength = 2,
                                                                                KeyboardType = KeyboardType.テンキー
                                                                      }]
                                                        }
                                         ]
                            }]);
        // 検査結果を取得
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId))
        .ReturnsAsync(new ExamResult
        {
            ConsultId = consultId,
            ExamItemDetailResults = [
                new ExamItemDetailResult{
                    ExamItemId = 1,
                    ExamItemDetailId = 1,
                    Value = "178"
                },
                new ExamItemDetailResult{
                    ExamItemId = 2,
                    ExamItemDetailId = 2,
                    Value = "70"
                },
                new ExamItemDetailResult{
                    ExamItemId = 23,
                    ExamItemDetailId = 230,
                    Value = "30"
                }
            ]
        });
        // 過去検査結果を取得
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate))
                                .ReturnsAsync(new PreviousResult
                                {
                                    ConsultId = consultId,
                                    ExamDate = new DateOnly(2023, 04, 10),
                                    ExamItemDetailResults = [
                                                                new ExamItemDetailResult{ ExamItemId = 1, ExamItemDetailId = 1, Value  = "175.4" },
                                                                new ExamItemDetailResult{ ExamItemId = 2, ExamItemDetailId = 2, Value  = "100.5" },
                                                                new ExamItemDetailResult{ ExamItemId = 23, ExamItemDetailId = 230, Value  = "20" }
                                                            ]
                                });
        // 検査項目明細マスタ一覧を取得
        _examItemRepositoryMock.Setup(x => x.GetExamItemDetailChildrenAsync(new int[] { 1, 230 })).ReturnsAsync([
            new ExamItemDetailChild{
                ExamItemDetailId = 1,
                Name = "身長",
                PositionNumber = 1,
                EquipmentLabel = "height",
                Unit = "cm",
                Type = ExamItemDetailType.入力,
                IntegerLength = 3,
                DecimalLength = 0,
                KeyboardType = KeyboardType.テンキー,
                Keyboards = [
                    new Keyboard{
                        OptionId = 1,
                        ExamItemDetailId = 1,
                        Value = "80"
                    },
                    new Keyboard{
                        OptionId = 2,
                        ExamItemDetailId = 1,
                        Value = "100"
                    },
                    new Keyboard{
                        OptionId = 3,
                        ExamItemDetailId = 1,
                        Value = "200"
                    }
                ],
                DetailOptions = []
            },
            new ExamItemDetailChild{
                ExamItemDetailId = 230,
                Name = "検査メニュー特記用明細",
                PositionNumber = 0,
                EquipmentLabel = "",
                Unit = "",
                Type = ExamItemDetailType.入力,
                IntegerLength = 3,
                DecimalLength = 0,
                KeyboardType = KeyboardType.テンキー,
                Keyboards = [],
                DetailOptions = []
            }
        ]);
        // 検査メニュー特記一覧を取得
        _examMenuRepositoryMock.Setup(x => x.GetMenuNotesAsync(examMenuId))
            .ReturnsAsync([
                new MenuNote{
                    MenuNoteId = 1,
                    Name = "身長",
                    ExamMenuId = examMenuId,
                    Suffix = "cm",
                    ConsultNotes = [],
                    ExamResults = [
                        new  MenuNoteExamResult{
                            ExamItemDetailId = 1,
                            SourceType = SourceType.今回値
                        },
                        new  MenuNoteExamResult{
                            ExamItemDetailId = 1,
                            SourceType = SourceType.前回値
                        }
                    ]
                },
                new MenuNote{
                    MenuNoteId = 2,
                    Name = "検査メニュー特記用",
                    ExamMenuId = 2,
                    Suffix = "",
                    ConsultNotes = [],
                    ExamResults = [
                        new  MenuNoteExamResult{
                            ExamItemDetailId = 230,
                            SourceType = SourceType.今回値
                        },
                        new  MenuNoteExamResult{
                            ExamItemDetailId = 230,
                            SourceType = SourceType.前回値
                        }
                    ]
                }
            ]);
        // 検査結果相関ルールマスタを取得する
        _examItemRepositoryMock.Setup(x => x.GetCorrelationRulesAsync(examMenuId))
        .ReturnsAsync([
            new CorrelationRule{
            CorrelationRuleId = 1,
            Name = "体重_前回差20kg以上",
            ExamMenuId = examMenuId,
            Priority = 1,
            TriggerType = RuleTriggerType.ThresholdExceeded,
            ErrorLevel = InputErrorLevel.異常,
            ExamItemId = 2,
            Message = "体重の前回差が20kg以上です。",
            Evaluations = [ new CorrelationRuleEvaluation { EvaluationValue = "20.0", VariableNumber = 1}],
            ExamItemDetails = [ new CorrelationRuleExamItemDetail{ ExamItemDetailId = 2, SourceType = SourceType.今回値, VariableNumber = 1},
                                new CorrelationRuleExamItemDetail{ ExamItemDetailId = 2, SourceType = SourceType.前回値, VariableNumber = 2}
                              ]
        }
        ]);
        // キーボード入力値リスト
        _examItemRepositoryMock.Setup(x => x.GetKeyboardOptionssAsync(new int[] { 1, 2 }))
        .ReturnsAsync([
            new Keyboard { ExamItemDetailId = 1, OptionId = 1, Value = "80"},
            new Keyboard { ExamItemDetailId = 1, OptionId = 2, Value = "100"},
            new Keyboard { ExamItemDetailId = 1, OptionId = 3, Value = "200"}
        ]);
        // 検査項目明細選択肢
        _examItemRepositoryMock.Setup(x => x.GetExamItemDetailOptionsAsync(new int[] { 1, 2 }))
        .ReturnsAsync([
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 1, Code = "001", Name = "痩せ"},
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 2, Code = "002", Name = "痩せ気味"},
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 3, Code = "003", Name = "標準"},
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 4, Code = "004", Name = "肥満気味"},
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 5, Code = "005", Name = "肥満"}
        ]);
        // 検査基準値範囲を取得
        var thresholdId1 = Guid.Parse("f1e54cf5-5aeb-4b1f-99f3-3dc984da6d00");
        var thresholdId2 = Guid.Parse("0838f508-f40d-4973-9f51-c43ab4f14805");
        var thresholdId3 = Guid.Parse("6537a6c5-c828-42e1-a1fb-4a76cbb41315");
        _consultRepositoryMock.Setup(x => x.GetExamNormalValueRangesAsync(consultId, new int[] { 1, 2 }))
        .ReturnsAsync([
            new ExamNormalValueRange("身長1", thresholdId1, 1, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(0, 100), InputErrorLevel.正常, 1),
            new ExamNormalValueRange("身長2", thresholdId2, 1, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(101, 150), InputErrorLevel.警告, 2),
            new ExamNormalValueRange("身長3", thresholdId3, 1, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(151, 200), InputErrorLevel.異常, 3),
            new ExamNormalValueRange("体重1", thresholdId1, 2, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(0, 50), InputErrorLevel.正常, 1),
            new ExamNormalValueRange("体重2", thresholdId2, 2, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(51, 150), InputErrorLevel.警告, 2),
            new ExamNormalValueRange("体重3", thresholdId3, 2, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(151, 200), InputErrorLevel.異常, 3)
        ]);
        // 検査中止を取得
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consultId))
        .ReturnsAsync(new ExamCancel
        {
            ConsultId = consultId,
            ExamItemDetailCancels = [
                new  ExamItemDetailCancel {
                    ExamItemId = 2, ExamItemDetailId = 2, CancelReasonId = 2
                }
            ]
        });
        // 検査依頼を取得
        _consultRepositoryMock.Setup(x => x.GetExamOrdersAsync(consultId))
        .ReturnsAsync(new ExamOrder
        {
            ConsultId = consultId,
            ExamItemDetailOrders = [
                new  ExamItemDetailOrder {
                    ExamItemId = 1, ExamItemDetailId = 1
                },
                new  ExamItemDetailOrder {
                    ExamItemId = 2, ExamItemDetailId = 2
                }
            ]
        });

        // 未受診の検査メニュー
        _consultRepositoryMock.Setup(x => x.GetUnexaminedConsultsAsync((new string[] { consultNumber })))
                              .ReturnsAsync(new List<UnexaminedConsult>(){
                                new(){
                                    ExamineeId = examineeId,
                                    ConsultId = consultId,
                                    ConsultNumber  = consultNumber,
                                    UnexaminedExamMenus = [
                                        new UnexaminedExamMenu(){
                                            ExamMenuId = 1,
                                            ExamMenuName = "メニュー名"
                                        }
                                    ]
                                }
                              });

        var expected = new APIModels.Responses.InputExamItems
        {
            ConsultNumber = "0001",
            Examinee =
                new APIModels.Responses.InputExamExaminee
                {
                    TicketNumber = "5963",
                    KanaName = "リョウビ　ハナコ",
                    Sex = (int)Sex.女,
                    ExamDateAge = 48
                },
            RelatedExamItems = [
                new APIModels.Responses.RelatedExamItem{ ExamItemName = "身長", ExamResult = "178(175.4)cm" },
                new APIModels.Responses.RelatedExamItem{ ExamItemName = "検査メニュー特記用", ExamResult = "30(20)" }
            ],
            IsComplete = false,
            ExamItemGroups = [
                new APIModels.Responses.ExamItemGroup{
                    Type = (int)ExamItemGroupType.数値,
                    ExamItems = [
                        new APIModels.Responses.InputExamItem{
                            PositionNumber = 1,
                            ExamItemId = 1,
                            Name = "身長",
                            ExamItemDetails = [
                                new APIModels.Responses.ExamItemDetail{
                                    PositionNumber = 1,
                                    ExamItemDetailId = 1,
                                    EquipmentLabel = "height",
                                    Name = "身長",
                                    HasOrder = true,
                                    CancelReasonId = null,
                                    Value = "178",
                                    PrevValue = "175.4",
                                    Unit = "cm",
                                    Type = (int)ExamItemDetailType.入力,
                                    IntegerLength = 3,
                                    DecimalLength = 2,
                                    Keyboard = new APIModels.Responses.Keyboard{
                                        KeyboardType = 1,
                                        Values = ["80", "100", "200"]
                                    },
                                    ExamItemDetailOptions = [],
                                    ExamNormalValueRanges = [
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.正常, MaxValue = 100, MinValue = 0},
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.警告, MaxValue = 150, MinValue = 101},
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.異常, MaxValue = 200, MinValue = 151}
                                    ]
                                }
                            ],
                            ExamRegistResults = []
                        },
                        new APIModels.Responses.InputExamItem{
                            PositionNumber = 2,
                            ExamItemId = 2,
                            Name = "体重",
                            ExamItemDetails = [
                                new APIModels.Responses.ExamItemDetail{
                                    PositionNumber = 2,
                                    ExamItemDetailId = 2,
                                    EquipmentLabel = "weight",
                                    Name = "体重",
                                    HasOrder = true,
                                    CancelReasonId = 2,
                                    Value = "70",
                                    PrevValue = "100.5",
                                    Unit = "kg",
                                    Type = (int)ExamItemDetailType.入力,
                                    IntegerLength = 3,
                                    DecimalLength = 2 ,
                                    Keyboard = new APIModels.Responses.Keyboard{
                                        KeyboardType = 1,
                                        Values = []
                                    },
                                    ExamItemDetailOptions = [
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 1, Code = "001", Name = "痩せ"},
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 2, Code = "002", Name = "痩せ気味"},
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 3, Code = "003", Name = "標準"},
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 4, Code = "004", Name = "肥満気味"},
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 5, Code = "005", Name = "肥満"}
                                    ],
                                    ExamNormalValueRanges = [
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.正常, MaxValue = 50, MinValue = 0},
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.警告, MaxValue = 150, MinValue = 51},
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.異常, MaxValue = 200, MinValue = 151}
                                    ]
                                }
                            ],
                            ExamRegistResults = [
                                new APIModels.Responses.ExamRegistResult { ErrorLevel = (int)InputErrorLevel.異常, Description = "体重の前回差が20kg以上です。"}
                            ]
                        }
                    ]
                }
            ]
        };

        // Act
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);
        var response = await usecase.GetInputExamItemsExamineeAsync(consultNumber, examMenuId);

        // Assert
        response.Should().BeEquivalentTo(expected);

    }

    [Fact]
    public async Task 検査結果を検証する()
    {
        // Arrange
        var consultNumber = "0006";
        var examMenuId = 1;
        var request = new ResultsRequest
        {
            ExamMenuId = examMenuId,
            ExamResults = [
                                new ResultRequest{
                                    ExamItemId = 1,
                                    ExamItemDetails = [
                                        new ExamItemDetailRequest {
                                            ExamItemDetailId = 1, Value = "175.3"
                                        }
                                    ]
                                },
                                new ResultRequest{
                                    ExamItemId = 2,
                                    ExamItemDetails = [
                                        new ExamItemDetailRequest {
                                            ExamItemDetailId = 2, Value = "70.5"
                                        }
                                    ]
                                }
                            ]
        };

        var examineeCode = "100002";
        var consultId = Guid.Parse("8d670eb8-d9f2-40b2-bfa6-cef6403be53d");
        var examineeId = Guid.Parse("4380755f-9398-4caa-aa2f-4ed7613d50d3");
        var placeScheduleId = Guid.Parse("531eb00c-1850-4d5e-9561-f24cdfd9a250");
        var organizationId = Guid.Parse("7bf76a3d-8bb4-41f5-8bd5-3b8fc8482511");
        var examDate = new DateOnly(2024, 10, 02);
        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber))
                            .ReturnsAsync(new Consult
                            {
                                ConsultId = consultId,
                                ConsultNumber = consultNumber,
                                Age = new Age(48, 3, 7),
                                ExamineeId = examineeId,
                                ProgressStatus = ConsultProgressStatus.来場待ち,
                                Note = "定期健康診断",
                                ExportStatus = ConsultResultExportStatus.未出力,
                                PlaceScheduleId = placeScheduleId,
                                TicketNumber = "1192"
                            });
        _examineeRepositoryMock.Setup(x => x.GetExamineeAsync(examineeId))
                            .ReturnsAsync(new Examinee
                            {
                                ExamineeId = examineeId,
                                ExamineeCode = examineeCode,
                                Name = "両備　花子",
                                KanaName = "リョウビ　ハナコ",
                                Sex = Sex.女,
                                Birthdate = new Birthdate(new DateOnly(1976, 6, 25)),
                                Affiliations = []
                            });
        // 会場日程IDを指定して会場日程を取得する
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(placeScheduleId))
                            .ReturnsAsync(new PlaceSchedule
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
                            });
        // 検査メニューに関連した検査項目情報を取得
        _examItemRepositoryMock.Setup(x => x.GetExamItemGroupsAsync(examMenuId))
                            .ReturnsAsync([ new ExamItemGroup{ ExamItemGroupId = 1, Type = ExamItemGroupType.数値,
                                                   ExamItems = [
                                                        new ExamItem { PositionNumber = 1,  ExamItemId = 1, Name = "身長",
                                                                       ExamItemDetails = [ new ExamItemDetail {
                                                                                PositionNumber = 1,
                                                                                ExamItemDetailId =1,
                                                                                EquipmentLabel = "height",
                                                                                Name = "身長",
                                                                                Unit = "cm",
                                                                                Type = ExamItemDetailType.入力,
                                                                                IntegerLength = 3,
                                                                                DecimalLength = 2,
                                                                                KeyboardType = KeyboardType.テンキー
                                                                      }]
                                                        },
                                                        new ExamItem { PositionNumber = 2,  ExamItemId = 2, Name = "体重",
                                                                       ExamItemDetails = [ new ExamItemDetail {
                                                                                PositionNumber = 2,
                                                                                ExamItemDetailId =2,
                                                                                EquipmentLabel = "weight",
                                                                                Name = "体重",
                                                                                Unit = "kg",
                                                                                Type = ExamItemDetailType.入力,
                                                                                IntegerLength = 3,
                                                                                DecimalLength = 2,
                                                                                KeyboardType = KeyboardType.テンキー
                                                                      }]
                                                        }
                                         ]
                            }]);
        // 検査結果を取得
        _consultRepositoryMock.Setup(x => x.GetExamResultsAsync(consultId))
        .ReturnsAsync(new ExamResult
        {
            ConsultId = consultId,
            ExamItemDetailResults = [
                new ExamItemDetailResult{
                    ExamItemId = 1,
                    ExamItemDetailId = 1,
                    Value = "178.5"
                },
                new ExamItemDetailResult{
                    ExamItemId = 2,
                    ExamItemDetailId = 2,
                    Value = "100.8"
                }
            ]
        });
        // 過去検査結果を取得
        _consultRepositoryMock.Setup(x => x.GetPreviousResultsAsync(consultId, examDate))
                                .ReturnsAsync(new PreviousResult
                                {
                                    ConsultId = consultId,
                                    ExamDate = new DateOnly(2023, 04, 10),
                                    ExamItemDetailResults = [
                                                                new ExamItemDetailResult{ ExamItemId = 1, ExamItemDetailId = 1, Value  = "175.4" },
                                                                new ExamItemDetailResult{ ExamItemId = 2, ExamItemDetailId = 2, Value  = "100.5" }
                                                            ]
                                });

        // 検査結果相関ルールマスタを取得する
        _examItemRepositoryMock.Setup(x => x.GetCorrelationRulesAsync(examMenuId))
        .ReturnsAsync([
            new CorrelationRule{
            CorrelationRuleId = 1,
            Name = "体重_前回差20kg以上",
            ExamMenuId = examMenuId,
            Priority = 1,
            TriggerType = RuleTriggerType.ThresholdExceeded,
            ErrorLevel = InputErrorLevel.異常,
            ExamItemId = 2,
            Message = "体重の前回差が20kg以上です。",
            Evaluations = [ new CorrelationRuleEvaluation { EvaluationValue = "20.0", VariableNumber = 1}],
            ExamItemDetails = [ new CorrelationRuleExamItemDetail{ ExamItemDetailId = 2, SourceType = SourceType.今回値, VariableNumber = 1},
                                new CorrelationRuleExamItemDetail{ ExamItemDetailId = 2, SourceType = SourceType.前回値, VariableNumber = 2}
                              ]
        }
        ]);
        // キーボード入力値リスト
        _examItemRepositoryMock.Setup(x => x.GetKeyboardOptionssAsync(new int[] { 1, 2 }))
        .ReturnsAsync([
            new Keyboard { ExamItemDetailId = 1, OptionId = 1, Value = "80"},
            new Keyboard { ExamItemDetailId = 1, OptionId = 2, Value = "100"},
            new Keyboard { ExamItemDetailId = 1, OptionId = 3, Value = "200"}
        ]);
        // 検査項目明細選択肢
        _examItemRepositoryMock.Setup(x => x.GetExamItemDetailOptionsAsync(new int[] { 1, 2 }))
        .ReturnsAsync([
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 1, Code = "001", Name = "痩せ"},
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 2, Code = "002", Name = "痩せ気味"},
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 3, Code = "003", Name = "標準"},
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 4, Code = "004", Name = "肥満気味"},
            new ExamItemDetailOption { ExamItemDetailId = 2, OrderNumber = 5, Code = "005", Name = "肥満"}
        ]);
        // 検査基準値範囲を取得
        var thresholdId1 = Guid.Parse("f1e54cf5-5aeb-4b1f-99f3-3dc984da6d00");
        var thresholdId2 = Guid.Parse("0838f508-f40d-4973-9f51-c43ab4f14805");
        var thresholdId3 = Guid.Parse("6537a6c5-c828-42e1-a1fb-4a76cbb41315");
        _consultRepositoryMock.Setup(x => x.GetExamNormalValueRangesAsync(consultId, new int[] { 1, 2 }))
        .ReturnsAsync([
            new ExamNormalValueRange("身長1", thresholdId1, 1, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(0, 100), InputErrorLevel.正常, 1),
            new ExamNormalValueRange("身長2", thresholdId2, 1, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(101, 150), InputErrorLevel.警告, 2),
            new ExamNormalValueRange("身長3", thresholdId3, 1, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(151, 200), InputErrorLevel.異常, 3),
            new ExamNormalValueRange("体重1", thresholdId1, 2, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(0, 50), InputErrorLevel.正常, 1),
            new ExamNormalValueRange("体重2", thresholdId2, 2, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(51, 150), InputErrorLevel.警告, 2),
            new ExamNormalValueRange("体重3", thresholdId3, 2, new TargetAge("00000","9999999"),
                                     new TargetSex(TargetSexType.両方), new ValueRange(151, 200), InputErrorLevel.異常, 3)
        ]);
        // 検査中止を取得
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consultId))
        .ReturnsAsync(new ExamCancel
        {
            ConsultId = consultId,
            ExamItemDetailCancels = [
                new  ExamItemDetailCancel {
                    ExamItemId = 2, ExamItemDetailId = 2, CancelReasonId = 2
                }
            ]
        });
        // 検査依頼を取得
        _consultRepositoryMock.Setup(x => x.GetExamOrdersAsync(consultId))
        .ReturnsAsync(new ExamOrder
        {
            ConsultId = consultId,
            ExamItemDetailOrders = [
                new  ExamItemDetailOrder {
                    ExamItemId = 1, ExamItemDetailId = 1
                },
                new  ExamItemDetailOrder {
                    ExamItemId = 2, ExamItemDetailId = 2
                }
            ]
        });

        var verifyExamItems = new APIModels.Responses.VerifyExamItems
        {
            ExamItemGroups = [
                new APIModels.Responses.ExamItemGroup
                {
                    Type = (int)ExamItemGroupType.数値 ,
                    ExamItems = [
                        new APIModels.Responses.InputExamItem{
                            PositionNumber = 1,
                            ExamItemId = 1,
                            Name = "身長",
                            ExamItemDetails = [
                                new APIModels.Responses.ExamItemDetail{
                                    PositionNumber = 1,
                                    ExamItemDetailId = 1,
                                    EquipmentLabel = "height",
                                    Name = "身長",
                                    HasOrder = true,
                                    CancelReasonId = null,
                                    Value = "178.5",
                                    PrevValue = "175.4",
                                    Unit = "cm",
                                    Type = (int)ExamItemDetailType.入力,
                                    IntegerLength = 3,
                                    DecimalLength = 2,
                                    Keyboard = new APIModels.Responses.Keyboard{
                                        KeyboardType = 1,
                                        Values = ["80", "100", "200"]
                                    },
                                    ExamItemDetailOptions = [],
                                    ExamNormalValueRanges = [
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.正常, MaxValue = 100, MinValue = 0},
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.警告, MaxValue = 150, MinValue = 101},
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.異常, MaxValue = 200, MinValue = 151}
                                    ]
                                }
                            ],
                            ExamRegistResults = []
                        },
                        new APIModels.Responses.InputExamItem{
                            PositionNumber = 2,
                            ExamItemId = 2,
                            Name = "体重",
                            ExamItemDetails = [
                                new APIModels.Responses.ExamItemDetail{
                                    PositionNumber = 2,
                                    ExamItemDetailId = 2,
                                    EquipmentLabel = "weight",
                                    Name = "体重",
                                    HasOrder = true,
                                    CancelReasonId = 2,
                                    Value = "100.8",
                                    PrevValue = "100.5",
                                    Unit = "kg",
                                    Type = (int)ExamItemDetailType.入力,
                                    IntegerLength = 3,
                                    DecimalLength = 2 ,
                                    Keyboard = new APIModels.Responses.Keyboard{
                                        KeyboardType = 1,
                                        Values = []
                                    },
                                    ExamItemDetailOptions = [
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 1, Code = "001", Name = "痩せ"},
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 2, Code = "002", Name = "痩せ気味"},
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 3, Code = "003", Name = "標準"},
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 4, Code = "004", Name = "肥満気味"},
                                        new APIModels.Responses.ExamItemDetailOption { OrderNumber = 5, Code = "005", Name = "肥満"}
                                    ],
                                    ExamNormalValueRanges = [
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.正常, MaxValue = 50, MinValue = 0},
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.警告, MaxValue = 150, MinValue = 51},
                                        new APIModels.Responses.ExamNormalValueRange { ErrorLevel = (int)InputErrorLevel.異常, MaxValue = 200, MinValue = 151}
                                    ]
                                }
                            ],
                            ExamRegistResults = [
                                new APIModels.Responses.ExamRegistResult { ErrorLevel = (int)InputErrorLevel.異常, Description = "体重の前回差が20kg以上です。"}
                            ]
                        }
                    ]
                }
            ]
        };
        // Act
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                         _staffIdentityProviderMock.Object);
        var response = await usecase.VerifyResults(consultNumber, request);

        // Assert
        response.Should().BeEquivalentTo(verifyExamItems);
    }

    [Fact]
    public async Task 未受診の検査メニューがない場合は空のリストを返す()
    {
        // Arrange
        var consultNumber = "12345";
        var examMenuId = 1;

        // 未受診の検査メニューなし
        _consultRepositoryMock.Setup(repo => repo.GetUnexaminedConsultsAsync(It.IsAny<string[]>()))
                              .ReturnsAsync(new List<UnexaminedConsult>());

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                 _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                 _staffIdentityProviderMock.Object);

        // Act
        var result = await usecase.ValidatePriorExamMenusAsync(consultNumber, examMenuId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task 現在の検査メニューに対する前提検査メニューの設定がない場合は空のリストを返す()
    {
        // Arrange
        var consultNumber = "12345";
        var examMenuId = 1;

        // 受診者に対する未受診の検査メニューあり
        var unexaminedConsult = new UnexaminedConsult()
        {
            ExamineeId = Guid.NewGuid(),
            ConsultId = Guid.NewGuid(),
            ConsultNumber = consultNumber,
            UnexaminedExamMenus = [new UnexaminedExamMenu { ExamMenuId = 2, ExamMenuName = "テストメニュー" }]
        };

        _consultRepositoryMock.Setup(repo => repo.GetUnexaminedConsultsAsync(It.IsAny<string[]>()))
                              .ReturnsAsync(new List<UnexaminedConsult> { unexaminedConsult });

        // 現在の検査メニューに対する前提検査メニューの設定なし
        _examMenuRepositoryMock.Setup(repo => repo.GetPriorExamMenusAsync(It.IsAny<int>()))
                               .ReturnsAsync((PriorExamMenu?)null);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                 _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                                 _staffIdentityProviderMock.Object);

        // Act
        var result = await usecase.ValidatePriorExamMenusAsync(consultNumber, examMenuId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task 前提検査メニューの設定があり未受診の検査メニューが含まれる場合はそれを返す()
    {
        // Arrange
        var consultNumber = "12345";
        var examMenuId = 1;

        // 受診者に対する未受診の検査メニューあり
        var unexaminedConsult = new UnexaminedConsult()
        {
            ExamineeId = Guid.NewGuid(),
            ConsultId = Guid.NewGuid(),
            ConsultNumber = consultNumber,
            UnexaminedExamMenus = [
                new UnexaminedExamMenu { ExamMenuId = 2, ExamMenuName = "テストメニュー2" },
                new UnexaminedExamMenu { ExamMenuId = 3, ExamMenuName = "テストメニュー3" },
                new UnexaminedExamMenu { ExamMenuId = 4, ExamMenuName = "テストメニュー4" }
            ]
        };

        _consultRepositoryMock.Setup(repo => repo.GetUnexaminedConsultsAsync(It.IsAny<string[]>()))
                              .ReturnsAsync([unexaminedConsult]);

        // 現在の検査メニューに対する前提検査メニューの設定あり
        var priorExamMenu = new PriorExamMenu(examMenuId, [2, 4, 6]);
        _examMenuRepositoryMock.Setup(repo => repo.GetPriorExamMenusAsync(It.IsAny<int>()))
                               .ReturnsAsync(priorExamMenu);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object,
                         _staffIdentityProviderMock.Object);


        // 前提検査メニューのうち、未受診の検査メニューは2と4
        var expectedResults = new List<ExamMenu>() {
            new(){MenuId = 2, MenuName = "テストメニュー2"},
            new(){MenuId = 4, MenuName = "テストメニュー4"}
         };

        // Act
        var result = await usecase.ValidatePriorExamMenusAsync(consultNumber, examMenuId);

        // Assert
        result.Should().BeEquivalentTo(expectedResults);
    }
}
