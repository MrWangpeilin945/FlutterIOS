using Moq;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
using Ryobi.Wellship.WebAPI.ExternalConnection.Enums;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class ExamNormalValueRangeUsecaseTests
{
    private readonly Mock<IExamNormalValueRangeRepository> _examNormalValueRangeRepositoryMock;
    private readonly Mock<IThresholdRepository> _thresholdRepositoryMock;
    private readonly Mock<IExternalExamItemDetailsRepository> _externalExamItemDetailsRepositoryMock;
    private readonly TimeProvider _timeProvider;

    public ExamNormalValueRangeUsecaseTests()
    {
        _examNormalValueRangeRepositoryMock = new Mock<IExamNormalValueRangeRepository>();
        _thresholdRepositoryMock = new Mock<IThresholdRepository>();
        _externalExamItemDetailsRepositoryMock = new Mock<IExternalExamItemDetailsRepository>();
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task 空値のチェック_名称で1件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009" }};
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Name", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009", MaxAge = "9990000", MinAge = "0000000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_基準値パターンCDで2件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "" })).ReturnsAsync(new List<ThresholdEntity> { });
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009" }};
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ThresholdCode", InputNote  = "UT2009"},
            new(){Code = "10001", Message = "指定されたThresholdCodeがシステム上に存在しません。Code:", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "", ExamItemDetailCode = "EEIDC2009", MaxAge = "9990000", MinAge = "0000000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_検査項目明細CDで2件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "" })).ReturnsAsync(new List<ExternalExamItemDetailEntity> {});
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ExamItemDetailCode", InputNote  = "UT2009"},
            new(){Code = "10001", Message = "指定されたExamItemDetailCodeがシステム上に存在しません。Code:", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "TC2009", ExamItemDetailCode = "", MaxAge = "9990000", MinAge = "0000000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_対象年齢上限で3件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009" }};
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。MaxAge", InputNote  = "UT2009"},
            new(){Code = "10006", Message = "値の形式が無効です。MaxAge:", InputNote  = "UT2009"},
            new(){Code = "10007", Message = "値の範囲が無効です。MinAge:0000000/MaxAge:", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009", MaxAge = "", MinAge = "0000000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 空値のチェック_対象年齢下限で2件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009" }};
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。MinAge", InputNote  = "UT2009"},
            new(){Code = "10006", Message = "値の形式が無効です。MinAge:", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009", MaxAge = "9990000", MinAge = "",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }    

    [Fact]
    public async Task 文字制限のチェック_対象年齢上限で1件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009" }};
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10006", Message = "値の形式が無効です。MaxAge:99-000z", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009", MaxAge = "99-000z", MinAge = "0000000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 文字制限のチェック_対象年齢下限で1件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009" }};
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10006", Message = "値の形式が無効です。MinAge:-00000z", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009", MaxAge = "9990000", MinAge = "-00000z",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 大小のチェック_対象年齢で1件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009" }};
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10007", Message = "値の範囲が無効です。MinAge:0400001/MaxAge:0400000", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009", MaxAge = "0400000", MinAge = "0400001",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 大小のチェック_基準値で1件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009" }};
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10007", Message = "値の範囲が無効です。MinValue:110/MaxValue:100", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009", MaxAge = "9990000", MinAge = "0000000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 110,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_基準値パターンCDで1件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(new List<ThresholdEntity> {});
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009" }};
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたThresholdCodeがシステム上に存在しません。Code:TC2009", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009", MaxAge = "9990000", MinAge = "0000000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task データのチェック_検査項目明細CDで1件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009" })).ReturnsAsync(new List<ExternalExamItemDetailEntity> {});
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたExamItemDetailCodeがシステム上に存在しません。Code:EEIDC2009", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009", MaxAge = "9990000", MinAge = "0000000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.両方 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

    [Fact]
    public async Task 重複キーのチェック_PKで2件のエラーが返る()
    {
        // Arrange
        // 基準値パターンIDの取得
        var thresholds = new List<ThresholdEntity> {
            new ThresholdEntity(){ ThresholdId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                   ThresholdCode = "TC2009", Name = "基準値パターン"} };
        _thresholdRepositoryMock.Setup(r => r.GetThresholdsByCodesAsync(new List<string> { "TC2009" })).ReturnsAsync(thresholds);
        // 検査項目明細IDの取得
        var externalExamItemDetails = new List<ExternalExamItemDetailEntity> {
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009-1" },
            new ExternalExamItemDetailEntity() { ExamItemDetailId = 2009, ExternalExamItemDetailCode = "EEIDC2009-2" }
        };
        _externalExamItemDetailsRepositoryMock.Setup(r => r.GetDetailsByCodesAsync(new List<string> { "EEIDC2009-1", "EEIDC2009-2" })).ReturnsAsync(externalExamItemDetails);
        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。ThresholdCode:TC2009/ExamItemDetailCode:EEIDC2009-1/TargetSex:男/MaxAge:0650000/MaxValue:100", InputNote  = "UT2009"},
            new(){Code = "10003", Message = "キー項目が重複しています。ThresholdCode:TC2009/ExamItemDetailCode:EEIDC2009-2/TargetSex:男/MaxAge:0650000/MaxValue:100", InputNote  = "UT2009"}
        };
        var usecase = new ExamNormalValueRangeUsecase(_examNormalValueRangeRepositoryMock.Object, _thresholdRepositoryMock.Object,
                                                      _externalExamItemDetailsRepositoryMock.Object, _timeProvider);
        var request = new List<ExamNormalValueRange>
        {
            new ExamNormalValueRange {
                    Name = "基準値1", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009-1", MaxAge = "0650000", MinAge = "0400000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.男 , MaxValue = 100, MinValue = 10,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                },
            new ExamNormalValueRange {
                    Name = "基準値2", ThresholdCode = "TC2009", ExamItemDetailCode = "EEIDC2009-2", MaxAge = "0650000", MinAge = "0350000",
                    TargetSex = Ryobi.Wellship.Core.Enums.TargetSexType.男 , MaxValue = 100, MinValue = 0,
                    ErrorLevel =  Ryobi.Wellship.Core.Enums.InputErrorLevel.正常, InputNote = "UT2009"
                }
        };
        // Act
        var errors = await usecase.StoreExamNormalValueRangeAsync(request);
        // Assert
        Assert.Equal(errors, expected);
    }

}
