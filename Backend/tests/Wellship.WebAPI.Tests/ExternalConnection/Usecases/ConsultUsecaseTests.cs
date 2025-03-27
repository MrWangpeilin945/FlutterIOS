using Moq;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
using Ryobi.Wellship.WebAPI.ExternalConnection.Enums;

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

    [Fact]
    public async Task 空値のチェック_連携キー_登録()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // *** 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ConnectionCode", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "連携キーが空値(登録)",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_連携キー_削除()
    {
        // Arrange
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ConnectionCode", InputNote  = "UT2004"},
            new(){Code = "10001", Message = "指定されたConnectionCodeがシステム上に存在しません。Code:", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "", ActionType = ActionType.削除, PlaceCode = "", TeamCode = "", ExamDate = "",
                    ConsultNumber = "", ExamineeCd = "", Note = "連携キーが空値(削除)",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_会場コード()
    {
        // Arrange
        // *** 会場コードに紐づく会場IDを取得する
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "" })).ReturnsAsync(new List<PlaceEntity>());
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // *** 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(new List<PlaceScheduleEntity>());
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。PlaceCode", InputNote  = "UT2004"}
            ,   new(){Code = "10001", Message = "指定されたPlaceCodeがシステム上に存在しません。Code:", InputNote  = "UT2004"}
            ,   new(){Code = "10001", Message = "指定されたPlaceScheduleがシステム上に存在しません。Code:PlaceCode:/TeamCode:T001/ExamDate:2024-10-02", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "会場コードが空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_班コード()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // *** 班コードに紐づく班IDを取得する
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "" })).ReturnsAsync(new List<TeamEntity>());
        // *** 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(new List<PlaceScheduleEntity>());
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。TeamCode", InputNote  = "UT2004"}
            ,   new(){Code = "10001", Message = "指定されたTeamCodeがシステム上に存在しません。Code:", InputNote  = "UT2004"}
            ,   new(){Code = "10001", Message = "指定されたPlaceScheduleがシステム上に存在しません。Code:PlaceCode:P001/TeamCode:/ExamDate:2024-10-02", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "班コードが空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_受診番号()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // *** 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ConsultNumber", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "", ExamineeCd = "E001", Note = "受診番号が空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_受診者コード()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // *** 受診者コードに紐づく受診者IDを取得する
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "" })).ReturnsAsync(new List<ExamineeEntity>());
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ExamineeCd", InputNote  = "UT2004"}
            ,   new(){Code = "10001", Message = "指定されたExamineeCdがシステム上に存在しません。Code:", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "", Note = "受診者コードが空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_過去検査結果_検査項目明細CD()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // *** 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string> { "" })).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。PreviousResults.ExamItemDetailCd", InputNote  = "UT2004"}
            ,   new(){Code = "10001", Message = "指定されたPreviousResults.ExamItemDetailCdがシステム上に存在しません。Code:", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "過去検査結果/検査項目明細CDが空値",
                    PreviousResults = new List<PreviousResult> { new PreviousResult(){ ExamItemDetailCd = "", ExamDate = DateOnly.Parse("2000-10-31"), Value = "100" } },
                    ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_過去検査結果_結果値()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // *** 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        var externalExamItemDetailEntities = new List<ExternalExamItemDetailEntity>{
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2004, ExternalExamItemDetailCode = "Prev001" }};
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string> { "Prev001" })).ReturnsAsync(externalExamItemDetailEntities);
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。PreviousResults.Value", InputNote  = "UT2004"} };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "過去検査結果/結果値が空値",
                    PreviousResults = new List<PreviousResult> { new PreviousResult(){ ExamItemDetailCd = "Prev001", ExamDate = DateOnly.Parse("2000-10-31"), Value = "" } },
                    ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_基準値判定_基準値判定コード()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // *** 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "" })).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ConsultThresholds.ThresholdCode", InputNote  = "UT2004"}
            ,   new(){Code = "10001", Message = "指定されたConsultThresholds.ThresholdCodeがシステム上に存在しません。Code:", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "基準値判定/基準値判定コードが空値",
                    PreviousResults = [],
                    ConsultThresholds = new List<ConsultThreshold> { new ConsultThreshold() { ThresholdCode = "", Priority = 1}},
                    ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_受診特記_検査特記コード()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // *** 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string> { "" })).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ConsultNotes.Code", InputNote  = "UT2004"}
            ,   new(){Code = "10001", Message = "指定されたConsultNotes.Codeがシステム上に存在しません。Code:", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "受診特記/検査特記コードが空値",
                    PreviousResults = [], ConsultThresholds = [],
                    ConsultNotes = new List<ConsultNote> { new ConsultNote() { Code = "", Note = "" }},
                    ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_検査項目明細依頼_検査項目明細CD()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // *** 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string> { "" })).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ExamItemDetailOrders.ExamItemDetailCd", InputNote  = "UT2004"}
            ,   new(){Code = "10001", Message = "指定されたExamItemDetailOrders.ExamItemDetailCdがシステム上に存在しません。Code:", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "検査項目明細依頼/検査項目明細CDが空値",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [],
                    ExamItemDetailOrders = new List<ExamItemDetailOrder> { new ExamItemDetailOrder() { ExamItemDetailCd = ""} },
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 重複キーのチェック_処理順()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001", "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001", "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"},
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-12"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001", "P001" }, new List<string> { "T001", "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02"), DateOnly.Parse("2024-10-12") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001", "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001", "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。SortNo: 1", InputNote  = "UT2004-01"},
            new(){Code = "10003", Message = "キー項目が重複しています。SortNo: 1", InputNote  = "UT2004-02"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "処理順が重複",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004-01"
                },
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-12",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "処理順が重複",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004-02"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 重複キーのチェック_基準値判定_基準値判定コード()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // *** 基準値パターンコードに紐づく基準値パターンIDを取得する
        var thresholdEntities = new List<ThresholdEntity> {
            new ThresholdEntity() { ThresholdId = Guid.Parse("f0000000-0000-0000-0000-000000000001"),
                                    ThresholdCode = "Th001", Name = "基準値パターン"}
        };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "Th001", "Th001" })).ReturnsAsync(thresholdEntities);
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。ConsultThresholds.ThresholdCode: Th001", InputNote  = "UT2004"}};
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "基準値判定/基準値判定コードが重複",
                    PreviousResults = [],
                    ConsultThresholds = new List<ConsultThreshold> {
                        new ConsultThreshold() { ThresholdCode = "Th001", Priority = 1},
                        new ConsultThreshold() { ThresholdCode = "Th001", Priority = 2}
                    },
                    ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 重複キーのチェック_基準値判定_優先度()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // *** 基準値パターンコードに紐づく基準値パターンIDを取得する
        var thresholdEntities = new List<ThresholdEntity> {
            new ThresholdEntity() { ThresholdId = Guid.Parse("f0000000-0000-0000-0000-000000000001"),
                                    ThresholdCode = "Th001", Name = "基準値パターン1"},
            new ThresholdEntity() { ThresholdId = Guid.Parse("f0000000-0000-0000-0000-000000000002"),
                                    ThresholdCode = "Th002", Name = "基準値パターン2"}
        };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "Th001", "Th002" })).ReturnsAsync(thresholdEntities);
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。ConsultThresholds.Priority: 1", InputNote  = "UT2004"}};
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "基準値判定/優先度が重複",
                    PreviousResults = [],
                    ConsultThresholds = new List<ConsultThreshold> {
                        new ConsultThreshold() { ThresholdCode = "Th001", Priority = 1},
                        new ConsultThreshold() { ThresholdCode = "Th002", Priority = 1}
                    },
                    ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 重複キーのチェック_受診特記_検査特記コード()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // *** 検査メニュー特記コードに紐づく情報を取得する
        var examMenuNoteCodeEntities = new List<ExamMenuNoteCodeEntity> { new ExamMenuNoteCodeEntity() { Code = "CT001", Name = "受診特記" } };
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string> { "CT001", "CT001" })).ReturnsAsync(examMenuNoteCodeEntities);
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。ConsultNotes.Code: CT001", InputNote  = "UT2004"}};
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "受診特記/検査特記コードが重複",
                    PreviousResults = [], ConsultThresholds = [],
                    ConsultNotes = new List<ConsultNote> {
                        new ConsultNote() { Code = "CT001", Note = "受診特記1" },
                        new ConsultNote() { Code = "CT001", Note = "受診特記2" }
                    },
                    ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 文字数のチェック_受診番号()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "12345678901234567890123456789012345678901234567890a" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10005", Message = "制限数を超えています。ConsultNumber:12345678901234567890123456789012345678901234567890a", InputNote  = "UT2004"}};
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "12345678901234567890123456789012345678901234567890a", ExamineeCd = "E001", Note = "受診番号文字数のチェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 文字制限のチェック_受診番号()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "123-456$_a#123-456$_a#" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10006", Message = "値の形式が無効です。ConsultNumber:123-456$_a#123-456$_a#", InputNote  = "UT2004"}};
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "123-456$_a#123-456$_a#", ExamineeCd = "E001", Note = "受診番号の形式のチェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 連携キーのチェック_受診番号()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        var externalConnectionCodeEntities = new List<ExternalConnectionCodeEntity> {
            new ExternalConnectionCodeEntity { ConsultId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                               ConsultNumber = "N001", ConnectionCode = "C100"}};
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(externalConnectionCodeEntities);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10002", Message = "指定されたConsultNumberが既に登録済みです。Code:N001", InputNote  = "UT2004"}};
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "受診番号連携キーのチェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_連携キー_削除()
    {
        // Arrange
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "D001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたConnectionCodeがシステム上に存在しません。Code:D001", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "D001", ActionType = ActionType.削除, PlaceCode = "", TeamCode = "", ExamDate = "",
                    ConsultNumber = "", ExamineeCd = "", Note = "連携キーのチェック(削除)",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_会場ID()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P999" })).ReturnsAsync(new List<PlaceEntity>());
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P999" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(new List<PlaceScheduleEntity>());
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたPlaceCodeがシステム上に存在しません。Code:P999", InputNote  = "UT2004"},
            new(){Code = "10001", Message = "指定されたPlaceScheduleがシステム上に存在しません。Code:PlaceCode:P999/TeamCode:T001/ExamDate:2024-10-02", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P999",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "会場IDのチェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_班ID()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T999" })).ReturnsAsync(new List<TeamEntity>());
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T999" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(new List<PlaceScheduleEntity>());
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたTeamCodeがシステム上に存在しません。Code:T999", InputNote  = "UT2004"},
            new(){Code = "10001", Message = "指定されたPlaceScheduleがシステム上に存在しません。Code:PlaceCode:P001/TeamCode:T999/ExamDate:2024-10-02", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T999", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "班IDのチェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_会場日程ID()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-31") })).ReturnsAsync(new List<PlaceScheduleEntity>());
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたPlaceScheduleがシステム上に存在しません。Code:PlaceCode:P001/TeamCode:T001/ExamDate:2024-10-31", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-31",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "会場日程IDのチェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_受診者ID()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E999" })).ReturnsAsync(new List<ExamineeEntity>());
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたExamineeCdがシステム上に存在しません。Code:E999", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E999", Note = "受診者IDのチェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_検査特記()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string> { "CN999" })).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたConsultNotes.Codeがシステム上に存在しません。Code:CN999", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "検査特記のチェック",
                    PreviousResults = [], ConsultThresholds = [],
                    ConsultNotes = new List<ConsultNote> { new ConsultNote() { Code = "CN999", Note = "検査特記" }},
                    ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_基準値パターン()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "Th999" })).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string>())).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたConsultThresholds.ThresholdCodeがシステム上に存在しません。Code:Th999", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "基準値パターンのチェック",
                    PreviousResults = [],
                    ConsultThresholds = new List<ConsultThreshold> {
                        new ConsultThreshold() { ThresholdCode = "Th999", Priority = 1}
                    },
                    ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_過去検査結果_検査項目明細CD()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string> { "Prev999" })).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたPreviousResults.ExamItemDetailCdがシステム上に存在しません。Code:Prev999", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "過去検査結果-検査項目明細CDのチェック",
                    PreviousResults = new List<PreviousResult> { new PreviousResult(){ ExamItemDetailCd = "Prev999", ExamDate = DateOnly.Parse("2023-10-31"), Value = "100" } },
                    ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 重複キーのチェック_過去検査結果_検査項目明細ID()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        var externalExamItemDetailEntities = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2004, ExternalExamItemDetailCode = "Prev001" },
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2004, ExternalExamItemDetailCode = "Prev002" }
        };
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string> { "Prev001", "Prev002" })).ReturnsAsync(externalExamItemDetailEntities);
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。Code:PreviousResults.ExamItemDetailCd:Prev001/PreviousResults.ExamDate:2023/10/20", InputNote  = "UT2004"},
            new(){Code = "10003", Message = "キー項目が重複しています。Code:PreviousResults.ExamItemDetailCd:Prev002/PreviousResults.ExamDate:2023/10/20", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "過去検査結果-検査項目明細IDの重複チェック",
                    PreviousResults = new List<PreviousResult> {
                        new PreviousResult(){ ExamItemDetailCd = "Prev001", ExamDate = DateOnly.Parse("2023-10-20"), Value = "100" },
                        new PreviousResult(){ ExamItemDetailCd = "Prev002", ExamDate = DateOnly.Parse("2023-10-20"), Value = "200" }
                    },
                    ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 重複キーのチェック_過去検査結果_検査項目明細CD()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        var externalExamItemDetailEntities = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2004, ExternalExamItemDetailCode = "Prev001" }
        };
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string> { "Prev001", "Prev001" })).ReturnsAsync(externalExamItemDetailEntities);
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。Code:PreviousResults.ExamItemDetailCd:Prev001/PreviousResults.ExamDate:2023/10/20", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "過去検査結果-検査項目明細IDの重複チェック",
                    PreviousResults = new List<PreviousResult> {
                        new PreviousResult(){ ExamItemDetailCd = "Prev001", ExamDate = DateOnly.Parse("2023-10-20"), Value = "100" },
                        new PreviousResult(){ ExamItemDetailCd = "Prev001", ExamDate = DateOnly.Parse("2023-10-20"), Value = "200" }
                    },
                    ConsultThresholds = [], ConsultNotes = [], ExamItemDetailOrders = [],
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_検査項目明細依頼_検査項目明細CD()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string> { "Order999" })).ReturnsAsync(new List<ExternalExamItemDetailEntity>());
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたExamItemDetailOrders.ExamItemDetailCdがシステム上に存在しません。Code:Order999", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "検査項目明細依頼-検査項目明細CDのチェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [],
                    ExamItemDetailOrders = new List<ExamItemDetailOrder> { new ExamItemDetailOrder() { ExamItemDetailCd = "Order999"} },
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 重複キーのチェック_検査項目明細依頼_検査項目明細ID()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        var externalExamItemDetailEntities = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2004, ExternalExamItemDetailCode = "Order001" },
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2004, ExternalExamItemDetailCode = "Order002" }
        };
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string> { "Order001", "Order002" })).ReturnsAsync(externalExamItemDetailEntities);
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。Code:ExamItemDetailOrders.ExamItemDetailCd:Order001", InputNote  = "UT2004"},
            new(){Code = "10003", Message = "キー項目が重複しています。Code:ExamItemDetailOrders.ExamItemDetailCd:Order002", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "検査項目明細依頼-検査項目明細IDの重複チェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [],
                    ExamItemDetailOrders = new List<ExamItemDetailOrder> {
                        new ExamItemDetailOrder() { ExamItemDetailCd = "Order001"},
                        new ExamItemDetailOrder() { ExamItemDetailCd = "Order002"}
                    },
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 重複キーのチェック_検査項目明細依頼_検査項目明細CD()
    {
        // Arrange
        // 会場コードに紐づく会場IDを取得する
        var placeEntities = new List<PlaceEntity> {
                    new PlaceEntity(){ PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001", Name = "会場"} };
        _placeRepositoryMock.Setup(r => r.GetPlaceInfoAsync(new List<string> { "P001" })).ReturnsAsync(placeEntities);
        // 班コードに紐づく班IDを取得する
        var teamEntities = new List<TeamEntity> {
                    new TeamEntity(){ TeamId = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001", Name = "班"} };
        _teamRepositoryMock.Setup(r => r.GetTeamInfoAsync(new List<string> { "T001" })).ReturnsAsync(teamEntities);
        // 会場コード、班コード、健診日に紐づく会場日程情報を取得する
        var placeScheduleEntities = new List<PlaceScheduleEntity> {
            new PlaceScheduleEntity(){ PlaceScheduleId = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
                                       PlaceId = Guid.Parse("a0000000-0000-0000-0000-000000000001"), PlaceCode = "P001",
                                       TeamId  = Guid.Parse("b0000000-0000-0000-0000-000000000001"), TeamCode = "T001" ,
                                       Status = 31, ExamDate = DateOnly.Parse("2024-10-02"), StartTime = "1500"} };
        _placeScheduleRepositoryMock.Setup(r => r.GetPlaceScheduleInfoAsync(new List<string> { "P001" }, new List<string> { "T001" },
                                                new List<DateOnly> { DateOnly.Parse("2024-10-02") })).ReturnsAsync(placeScheduleEntities);
        // 受診者コードに紐づく受診者IDを取得する
        var examineeEntities = new List<ExamineeEntity> {
            new ExamineeEntity() { ExamineeId = Guid.Parse("e0000000-0000-0000-0000-000000000001"), ExamineeCode= "E001",
                                   Name = "受診　太郎", KanaName = "ジュシン　タロウ", Sex = 1, Birthdate = DateOnly.Parse("1975-08-15")} };
        _examineeRepositoryMock.Setup(r => r.GetExamineeInfoAsync(new List<string> { "E001" })).ReturnsAsync(examineeEntities);
        // 検査メニュー特記コードに紐づく情報を取得する
        _consultRepositoryMock.Setup(r => r.GetExamMenuNodeCodeInfoAsync(new List<string>())).ReturnsAsync(new List<ExamMenuNoteCodeEntity>());
        // 基準値パターンコードに紐づく基準値パターンIDを取得する
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string>())).ReturnsAsync(new List<ThresholdEntity>());
        // 検査項目明細CDに紐づく外部検査項目明細IDを取得する
        var externalExamItemDetailEntities = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2004, ExternalExamItemDetailCode = "Order001" }
        };
        _consultRepositoryMock.Setup(r => r.GetExternalExamItemDetailInfoAsync(new List<string> { "Order001", "Order001" })).ReturnsAsync(externalExamItemDetailEntities);
        // 連携キーに紐づく外部連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetExternalConnectionCodeAsync(new List<string> { "C001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // 受診番号に紐づく連携キーを取得する
        _consultRepositoryMock.Setup(r => r.GetConsultExternalConnectionCodeAsync(new List<string> { "N001" })).ReturnsAsync(new List<ExternalConnectionCodeEntity>());
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。Code:ExamItemDetailOrders.ExamItemDetailCd:Order001", InputNote  = "UT2004"}
        };
        var usecase = new ConsultUsecase(_consultRepositoryMock.Object, _teamRepositoryMock.Object,
                                         _placeRepositoryMock.Object, _placeScheduleRepositoryMock.Object,
                                         _examineeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                         _timeProvider);
        var request = new List<Consult>
        {
            new Consult {
                    SortNo= 1, ConnectionCode = "C001", ActionType = ActionType.登録, PlaceCode = "P001",TeamCode = "T001", ExamDate = "2024-10-02",
                    ConsultNumber = "N001", ExamineeCd = "E001", Note = "検査項目明細依頼-検査項目明細IDの重複チェック",
                    PreviousResults = [], ConsultThresholds = [], ConsultNotes = [],
                    ExamItemDetailOrders = new List<ExamItemDetailOrder> {
                        new ExamItemDetailOrder() { ExamItemDetailCd = "Order001"},
                        new ExamItemDetailOrder() { ExamItemDetailCd = "Order001"}
                    },
                    InputNote = "UT2004"
                }
        };
        // Act
        var errors = await usecase.StoreConsultAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

}
