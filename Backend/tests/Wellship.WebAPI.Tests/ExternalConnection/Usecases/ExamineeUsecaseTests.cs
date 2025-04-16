using Moq;
using FluentAssertions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class ExamineeUsecaseTests
{
    private readonly Mock<IOrganizationRepository> _organizationRepositoryMock;
    private readonly Mock<IExamineeRepository> _examineeRepositoryMock;
    private readonly TimeProvider _timeProvider;

    public ExamineeUsecaseTests()
    {
        _organizationRepositoryMock = new Mock<IOrganizationRepository>();
        _examineeRepositoryMock = new Mock<IExamineeRepository>();
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task 空値のチェック_受診者コードで1件のエラーが返る()
    {
        // Arrange
        // 団体コードを元に団体情報を取得する
        var organizationEntities = new List<OrganizationEntity> { 
            new OrganizationEntity() { OrganizationId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                     OrganizationCode = "0001", Name = "団体Ａ" }
        };
        _organizationRepositoryMock.Setup(x => x.GetOrganizationInfoAsync(new List<string> {"0001"})).ReturnsAsync(organizationEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。ExamineeCode", InputNote  = "UT2001"}
        };
        var usecase = new ExamineeUsecase(_organizationRepositoryMock.Object,  _examineeRepositoryMock.Object, _timeProvider);
        var request = new List<Examinee>
        {
            new Examinee { ExamineeCode = "", Name = "岡山　一郎", KanaName = "オカヤマ　イチロウ", Sex = Ryobi.Wellship.Core.Enums.Sex.男, Birthdate = DateOnly.Parse("1960-10-10"), 
                           Affiliations = new List<Affiliation>{ new Affiliation { OrganizationCode = "0001"} },  InputNote = "UT2001" }
        };
        // Act
        var errors = await usecase.StoreExamineesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_氏名で1件のエラーが返る()
    {
        // Arrange
        // 団体コードを元に団体情報を取得する
        var organizationEntities = new List<OrganizationEntity> { 
            new OrganizationEntity() { OrganizationId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                     OrganizationCode = "0001", Name = "団体Ａ" }
        };
        _organizationRepositoryMock.Setup(x => x.GetOrganizationInfoAsync(new List<string> {"0001"})).ReturnsAsync(organizationEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Name", InputNote  = "UT2001"}
        };
        var usecase = new ExamineeUsecase(_organizationRepositoryMock.Object,  _examineeRepositoryMock.Object, _timeProvider);
        var request = new List<Examinee>
        {
            new Examinee { ExamineeCode = "E0001", Name = "", KanaName = "オカヤマ　イチロウ", Sex = Ryobi.Wellship.Core.Enums.Sex.男, Birthdate = DateOnly.Parse("1960-10-10"), 
                           Affiliations = new List<Affiliation>{ new Affiliation { OrganizationCode = "0001"} },  InputNote = "UT2001" }
        };
        // Act
        var errors = await usecase.StoreExamineesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_カナ氏名で1件のエラーが返る()
    {
        // Arrange
        // 団体コードを元に団体情報を取得する
        var organizationEntities = new List<OrganizationEntity> { 
            new OrganizationEntity() { OrganizationId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                     OrganizationCode = "0001", Name = "団体Ａ" }
        };
        _organizationRepositoryMock.Setup(x => x.GetOrganizationInfoAsync(new List<string> {"0001"})).ReturnsAsync(organizationEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。KanaName", InputNote  = "UT2001"}
        };
        var usecase = new ExamineeUsecase(_organizationRepositoryMock.Object,  _examineeRepositoryMock.Object, _timeProvider);
        var request = new List<Examinee>
        {
            new Examinee { ExamineeCode = "E0001", Name = "岡山　一郎", KanaName = "", Sex = Ryobi.Wellship.Core.Enums.Sex.男, Birthdate = DateOnly.Parse("1960-10-10"), 
                           Affiliations = new List<Affiliation>{ new Affiliation { OrganizationCode = "0001"} },  InputNote = "UT2001" }
        };
        // Act
        var errors = await usecase.StoreExamineesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_所属団体コードで2件のエラーが返る()
    {
        // Arrange
        // 団体コードを元に団体情報を取得する
        _organizationRepositoryMock.Setup(x => x.GetOrganizationInfoAsync(new List<string> {""})).ReturnsAsync(new List<OrganizationEntity> {});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Affiliations.OrganizationCode", InputNote  = "UT2001"},
            new(){Code = "10001", Message = "指定されたOrganizationCodeがシステム上に存在しません。Code:", InputNote  = "UT2001"}
        };
        var usecase = new ExamineeUsecase(_organizationRepositoryMock.Object,  _examineeRepositoryMock.Object, _timeProvider);
        var request = new List<Examinee>
        {
            new Examinee { ExamineeCode = "E0001", Name = "岡山　一郎", KanaName = "オカヤマ　イチロウ", Sex = Ryobi.Wellship.Core.Enums.Sex.男, Birthdate = DateOnly.Parse("1960-10-10"), 
                           Affiliations = new List<Affiliation>{ new Affiliation { OrganizationCode = ""} },  InputNote = "UT2001" }
        };
        // Act
        var errors = await usecase.StoreExamineesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 重複キーのチェック_受診者コードで2件のエラーが返る()
    {
        // Arrange
        // 団体コードを元に団体情報を取得する
        var organizationEntities = new List<OrganizationEntity> { 
            new OrganizationEntity() { OrganizationId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                     OrganizationCode = "0001", Name = "団体Ａ" }
        };
        _organizationRepositoryMock.Setup(x => x.GetOrganizationInfoAsync(new List<string> {"0001"})).ReturnsAsync(organizationEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。ExamineeCode:E0001", InputNote  = "UT2001-01"},
            new(){Code = "10003", Message = "キー項目が重複しています。ExamineeCode:E0001", InputNote  = "UT2001-02"}
        };
        var usecase = new ExamineeUsecase(_organizationRepositoryMock.Object,  _examineeRepositoryMock.Object, _timeProvider);
        var request = new List<Examinee>
        {
            new Examinee { ExamineeCode = "E0001", Name = "岡山　一郎", KanaName = "オカヤマ　イチロウ", Sex = Ryobi.Wellship.Core.Enums.Sex.男, Birthdate = DateOnly.Parse("1960-10-10"), 
                           Affiliations = new List<Affiliation>{ new Affiliation { OrganizationCode = "0001"} },  InputNote = "UT2001-01" },
            new Examinee { ExamineeCode = "E0001", Name = "岡山　二郎", KanaName = "オカヤマ　ジロウ", Sex = Ryobi.Wellship.Core.Enums.Sex.男, Birthdate = DateOnly.Parse("1970-07-31"), 
                           Affiliations = new List<Affiliation>{ new Affiliation { OrganizationCode = "0001"} },  InputNote = "UT2001-02" }
        };
        // Act
        var errors = await usecase.StoreExamineesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 重複キーのチェック_所属団体コードで1件のエラーが返る()
    {
        // Arrange
        // 団体コードを元に団体情報を取得する
        var organizationEntities = new List<OrganizationEntity> { 
            new OrganizationEntity() { OrganizationId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                                     OrganizationCode = "0001", Name = "団体Ａ" }
        };
        _organizationRepositoryMock.Setup(x => x.GetOrganizationInfoAsync(new List<string> {"0001"})).ReturnsAsync(organizationEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。Affiliations.OrganizationCode:0001", InputNote  = "UT2001"}
        };
        var usecase = new ExamineeUsecase(_organizationRepositoryMock.Object,  _examineeRepositoryMock.Object, _timeProvider);
        var request = new List<Examinee>
        {
            new Examinee { ExamineeCode = "E0001", Name = "岡山　一郎", KanaName = "オカヤマ　イチロウ", Sex = Ryobi.Wellship.Core.Enums.Sex.男, Birthdate = DateOnly.Parse("1960-10-10"), 
                           Affiliations = new List<Affiliation>{ new Affiliation { OrganizationCode = "0001"}, new Affiliation { OrganizationCode = "0001"} },  InputNote = "UT2001" }
        };
        // Act
        var errors = await usecase.StoreExamineesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task データのチェック_所属団体コードで1件のエラーが返る()
    {
        // Arrange
        // 団体コードを元に団体情報を取得する
        _organizationRepositoryMock.Setup(x => x.GetOrganizationInfoAsync(new List<string> {"0001"})).ReturnsAsync(new List<OrganizationEntity> {});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10001", Message = "指定されたOrganizationCodeがシステム上に存在しません。Code:0001", InputNote  = "UT2001"}
        };
        var usecase = new ExamineeUsecase(_organizationRepositoryMock.Object,  _examineeRepositoryMock.Object, _timeProvider);
        var request = new List<Examinee>
        {
            new Examinee { ExamineeCode = "E0001", Name = "岡山　一郎", KanaName = "オカヤマ　イチロウ", Sex = Ryobi.Wellship.Core.Enums.Sex.男, Birthdate = DateOnly.Parse("1960-10-10"), 
                           Affiliations = new List<Affiliation>{ new Affiliation { OrganizationCode = "0001"} },  InputNote = "UT2001" }
        };
        // Act
        var errors = await usecase.StoreExamineesAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

}
