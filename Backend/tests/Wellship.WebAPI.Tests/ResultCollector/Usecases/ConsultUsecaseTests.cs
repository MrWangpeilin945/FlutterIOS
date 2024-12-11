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

    public ConsultUsecaseTests()
    {
        _examineeRepositoryMock = new Mock<IExamineeRepository>();
        _consultRepositoryMock = new Mock<IConsultRepository>();
        _examMenuRepositoryMock = new Mock<IExamMenuRepository>();
        _examItemRepositoryMock = new Mock<IExamItemRepository>();
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
    }

    [Fact]
    public async Task 受診番号が存在する場合に例外がスローされない()
    {
        // Arrange
        _consultRepositoryMock.Setup(x => x.ConsultExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object);
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
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object);
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
                             ConsultId = 1,
                             ConsultNumber = "0001",
                             ExamineeId = 1,
                             ProgressStatus = ConsultProgressStatus.検査中,
                             ExportStatus = ConsultResultExportStatus.未出力,
                             PlaceScheduleId = 1,
                             TicketNumber = "1029"
                         });

        _consultRepositoryMock.Setup(x => x.GetUnexaminedConsultsAsync(new[] { consultNumber }))
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

        _examineeRepositoryMock.Setup(x => x.GetExamineeAsync(1))
                          .ReturnsAsync(new WebAPI.ResultCollector.Domain.Models.Examinee
                          {
                              ExamineeId = 1,
                              ExamineeCode = "10001",
                              Name = "両備　太郎",
                              KanaName = "リョウビ　タロウ",
                              Sex = Sex.男,
                              Birthdate = new Birthdate("19991129")
                          });

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

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
            PlaceScheduleId = 1,
            ConsultId = 1,
            ExamineeId = 1,
            TicketNumber = "1029"
        });
        _examineeRepositoryMock.Setup(x => x.GetExamineeAsync(1))
                          .ReturnsAsync(new WebAPI.ResultCollector.Domain.Models.Examinee
                          {
                              ExamineeId = 1,
                              ExamineeCode = "10001",
                              Name = "両備　太郎",
                              KanaName = "リョウビ　タロウ",
                              Sex = Sex.男,
                              Birthdate = new Birthdate("19991129")
                          });

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

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
            ConsultId = 1,
            ConsultNumber = "0001",
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            PlaceScheduleId = 1,
            ExamineeId = 1,
            TicketNumber = "1029"
        };

        // すでに存在するキャンセルレコード
        var examCancels = new ExamCancel()
        {
            ConsultId = 1,
            ExamItemDetailCancels = [
                new ExamItemDetailCancel() {ExamItemDetailId = 1, ExamItemId = 1, CancelReasonId = 1},
                new ExamItemDetailCancel() {ExamItemDetailId = 711, ExamItemId = 71, CancelReasonId = 3},
                new ExamItemDetailCancel() {ExamItemDetailId = 712, ExamItemId = 71, CancelReasonId = 3},
            ]
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consult.ConsultId)).ReturnsAsync(examCancels);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

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
            ConsultId = 1,
            ConsultNumber = "0001",
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            PlaceScheduleId = 1,
            ExamineeId = 1,
            TicketNumber = "1029"
        };

        // すでに存在するキャンセルレコード
        var examCancel = new ExamCancel()
        {
            // キャンセルレコードなし
            ConsultId = 1,
            ExamItemDetailCancels = []
        };

        _consultRepositoryMock.Setup(x => x.GetConsultAsync(consultNumber)).ReturnsAsync(consult);
        _consultRepositoryMock.Setup(x => x.GetExamCancelsAsync(consult.ConsultId)).ReturnsAsync(examCancel);

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _examineeRepositoryMock.Object, _examMenuRepositoryMock.Object,
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

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
        var consultId = 2;
        var examDate = new DateOnly(2024, 11, 30);

        var consult = new Consult()
        {
            ConsultId = consultId,
            ConsultNumber = consultNumber,
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            ExamineeId = 2,
            PlaceScheduleId = 1,
            TicketNumber = "1002"
        };

        var placeSchedule = new WebAPI.ResultCollector.Domain.Models.PlaceSchedule()
        {
            Id = 1,
            Place = new WebAPI.ResultCollector.Domain.Models.Place() { Id = 1, Code = "001", Name = "会場1", OrderNumber = 1 },
            Team = new Team() { Id = 1, Code = "001", Name = "1班", OrderNumber = 1 },
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
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(1)).ReturnsAsync(placeSchedule);
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
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

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
        var consultId = 2;
        var examDate = new DateOnly(2024, 11, 30);

        var consult = new Consult()
        {
            ConsultId = consultId,
            ConsultNumber = consultNumber,
            ProgressStatus = ConsultProgressStatus.検査中,
            ExportStatus = ConsultResultExportStatus.未出力,
            ExamineeId = 2,
            PlaceScheduleId = 1,
            TicketNumber = "1002"
        };

        var placeSchedule = new WebAPI.ResultCollector.Domain.Models.PlaceSchedule()
        {
            Id = 1,
            Place = new WebAPI.ResultCollector.Domain.Models.Place() { Id = 1, Code = "001", Name = "会場1", OrderNumber = 1 },
            Team = new Team() { Id = 1, Code = "001", Name = "1班", OrderNumber = 1 },
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
                CorrelationRuleId = 1,
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
        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleAsync(1)).ReturnsAsync(placeSchedule);
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
                                         _examItemRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

        // Act
        var errors = await usecase.ValidateCorrelationRuleAsync(consultNumber, resultsRequest);

        // Assert
        errors.Should().BeEquivalentTo(expected);
    }
}
