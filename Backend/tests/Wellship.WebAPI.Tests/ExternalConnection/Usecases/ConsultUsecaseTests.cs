
using Moq;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class ConsultUsecaseTests
{
    private readonly Mock<IConsultRepository> _consultRepositoryMock;
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IPlaceRepository> _placeRepositoryMock;
    private readonly Mock<IPlaceScheduleRepository> _placeScheduleRepositoryMock;
    private readonly Mock<IExamineeRepository> _examineeRepositoryMock;
    private readonly Mock<IThresholdRepository> _thresholdRepositoryMock;
    private readonly TimeProvider _timeProvider;


    public ConsultUsecaseTests()
    {
        _consultRepositoryMock = new Mock<IConsultRepository>();
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _placeRepositoryMock = new Mock<IPlaceRepository>();
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
        _examineeRepositoryMock = new Mock<IExamineeRepository>();
        _thresholdRepositoryMock = new Mock<IThresholdRepository>();
        _timeProvider = TimeProvider.System;
    }

    private void setupMock()
    {
        // 会場コードに紐づく会場IDを取得する
        var placeCodes = new List<string> { "P001" };
        var plcaseEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(placeCodes)).ReturnsAsync(plcaseEntities);

        // 班コードに紐づく班IDを取得する
        var teamCodes = new List<string> { "T001" };
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(teamCodes)).ReturnsAsync(teamEntities);

        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var examDates = new List<DateOnly> { DateOnly.Parse("2024-10-02") };
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(placeCodes, teamCodes, examDates)).ReturnsAsync(placeScheduleEntities);

        // 受診者コードに紐づく受診者IDを取得する
        var examineeCodes = new List<string> { "E001" };
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(examineeCodes)).ReturnsAsync(examineeEntities);

        // 検査メニュー特記コードに紐づく情報を取得する
        var consultNoteCodes = new List<string> { "N001" };
        var examMenuNoteCodeEntities = new List<ExamMenuNoteCodeEntity> {
            new ExamMenuNoteCodeEntity() {Code = "N001", Name = "検査特記名"} };
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(consultNoteCodes)).ReturnsAsync(examMenuNoteCodeEntities);

        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        var thresholdCodes = new List<string> { "TC001" };
        var thresholdEntities = new List<ThresholdEntity> {
            new ThresholdEntity() {ThresholdId = Guid.Parse("f0000000-0000-0000-0000-000000000001"), ThresholdCode = "TC001", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(thresholdCodes)).ReturnsAsync(thresholdEntities);

        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        var detailCodes = new List<string> { "OD001" };
        var externalExamItemDetailEntities = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 1, ExternalExamItemDetailCode = "OD001"} };
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(detailCodes)).ReturnsAsync(externalExamItemDetailEntities);

        // 連携キーに紐づく外部連携キーを取得する
        var connectionCodes = new List<string> { "C002", "C003", "C004", "C005", "C006", "C007", "C008", "C009" };
        var externalConnectionCodeEntities1 = new List<ExternalConnectionCodeEntity> {
        };
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(connectionCodes)).ReturnsAsync(externalConnectionCodeEntities1);

        // 受診番号に紐づく連携キーを取得する
        var consultNumbers = new List<string> { "N001", "N002", "N003", "N004", "N005", "N006", "N007", "N008", "N009" };
        var externalConnectionCodeEntities2 = new List<ExternalConnectionCodeEntity> {
            new ExternalConnectionCodeEntity() { ConsultId = Guid.Parse("00000000-0000-0000-0000-000000000001"), ConnectionCode = "0000", ConsultNumber = "0000"},
            new ExternalConnectionCodeEntity() { ConsultId = Guid.Parse("00000000-0000-0000-0000-000000000002"), ConnectionCode = "0000", ConsultNumber = "0000"},
            new ExternalConnectionCodeEntity() { ConsultId = Guid.Parse("00000000-0000-0000-0000-000000000003"), ConnectionCode = "0000", ConsultNumber = "0000"}
        };
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(consultNumbers)).ReturnsAsync(externalConnectionCodeEntities2);
    }

    [Fact]
    public async Task 必須項目の空値のチェック()
    {
        // Arrange
        setupMock();    

        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "", ActionType = 0,
                    PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "連携キーが空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT001"
                },
            new Consult {
                    SortNo= 2, ConnectionCode = "C002", ActionType = 0,
                    PlaceCode = "", TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N002", ExamineeCd = "E001", Note = "会場コードが空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT002"
                },
            new Consult {
                    SortNo= 3, ConnectionCode = "C003", ActionType = 0,
                    PlaceCode = "P001", TeamCode = "", ExamDate = "2024-10-02",
                    ConsultNumber = "N003", ExamineeCd = "E001", Note = "班コードが空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT003"
                },
            new Consult {
                    SortNo= 4, ConnectionCode = "C004", ActionType = 0,
                    PlaceCode = "P001", TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "", ExamineeCd = "E001", Note = "受診番号が空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT004"
                },
            new Consult {
                    SortNo= 5, ConnectionCode = "C005", ActionType = 0,
                    PlaceCode = "P001", TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "", ExamineeCd = "E001", Note = "受診者コードが空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT005"
                },
            new Consult {
                    SortNo= 6, ConnectionCode = "C006", ActionType = 0,
                    PlaceCode = "P001", TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "", ExamineeCd = "E001", Note = "過去検査結果の検査項目明細CDと結果値が空値",
                    PreviousResults = new List<PreviousResult> { new PreviousResult(){ ExamItemDetailCd = "", ExamDate = DateOnly.Parse("2000-10-31"), Value = "" } },
                    ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT006"
                },
            new Consult {
                    SortNo= 7, ConnectionCode = "C007", ActionType = 0,
                    PlaceCode = "P001", TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "", ExamineeCd = "E001", Note = "基準値判定の基準値判定コードが空値",
                    PreviousResults = [],
                    ConsultThresholds = new List<ConsultThreshold> { new ConsultThreshold() { ThresholdCode = "", Priority = 1}},
                    ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT007"
                },
            new Consult {
                    SortNo= 8, ConnectionCode = "C008", ActionType = 0,
                    PlaceCode = "P001", TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "", ExamineeCd = "E001", Note = "受診特記の検査特記コードが空値",
                    PreviousResults = [], ConsultThresholds = [],
                    ConsultNotes = new List<ConsultNote> { new ConsultNote() { Code = "", Note = "" }}, 
                    ExamItemDetailOrders = [],
                    InputNote = "UT008"
                },
            new Consult {
                    SortNo= 9, ConnectionCode = "C009", ActionType = 0,
                    PlaceCode = "P001", TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "", ExamineeCd = "E001", Note = "受診特記の検査特記コードが空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], 
                    ExamItemDetailOrders = new List<ExamItemDetailOrder> { new ExamItemDetailOrder() { ExamItemDetailCd = ""} },
                    InputNote = "UT009"
                }
        };
        // Act & Assert
        await usecase.StoreConsultAsync(request);
        /*
                await usecase.Invoking(x => x.VerifyConsultNumberAsync(request))
                              .Should().NotThrowAsync<ConsultNumberNotFoundException>();
                              */
    }

}
