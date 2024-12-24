using FluentAssertions;

using Moq;

using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
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

    public ConsultUsecaseTests()
    {
        _examineeRepositoryMock = new Mock<IExamineeRepository>();
        _consultRepositoryMock = new Mock<IConsultRepository>();
        _examMenuRepositoryMock = new Mock<IExamMenuRepository>();
        _examItemRepositoryMock = new Mock<IExamItemRepository>();
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
        _resultRepositoryMock = new Mock<IResultRepository>();
    }

    [Fact]
    public async Task 受診番号が存在する場合に例外がスローされない()
    {
        // Arrange
        _consultRepositoryMock.Setup(x => x.ConsultExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);
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
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);
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
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);

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
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);

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

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consult.ConsultId)).ReturnsAsync(examCancels);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);

        // Act
        await usecase.RegisterExecutionsAsync(consultNumber, request);

        // Assert
        _consultRepositoryMock.Verify(x => x.RemoveExamCancelsAsync(consult.ConsultId, It.IsAny<int[]>()), Times.Once);
        _consultRepositoryMock.Verify(x => x.SaveExamCancelsAsync(consult.ConsultId, It.IsAny<ExamItemCancel[]>()), Times.Once);
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

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consult.ConsultId)).ReturnsAsync(examCancel);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);

        // Act
        await usecase.RegisterExecutionsAsync(consultNumber, request);

        // Assert
        _consultRepositoryMock.Verify(x => x.RemoveExamCancelsAsync(consult.ConsultId, It.IsAny<int[]>()), Times.Once());
        _consultRepositoryMock.Verify(x => x.SaveExamCancelsAsync(consult.ConsultId, It.IsAny<ExamItemCancel[]>()), Times.Once);
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
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);

        // Act
        var errors = await usecase.ValidateCorrelationRuleAsync(consultNumber, resultsRequest);

        // Assert
        errors.Should().BeEquivalentTo(expected);
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
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);

        // Act
        var errors = await usecase.ValidateCorrelationRuleAsync(consultNumber, resultsRequest);

        // Assert
        errors.Should().BeEquivalentTo(expected);
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
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);
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
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object, _resultRepositoryMock.Object);
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
