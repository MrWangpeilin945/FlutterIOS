
using Moq;
using FluentAssertions;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Wellship.WebAPI.Tests.ExternalConnection.Usecases;

public class StaffUsecaseTests
{
    private readonly Mock<IStaffRepository> _staffRepositoryMock;
    private readonly TimeProvider _timeProvider;

    public StaffUsecaseTests()
    {
        _staffRepositoryMock = new Mock<IStaffRepository>();
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public async Task 空値のチェック_職員コードで1件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User01" } )).ReturnsAsync(new List<StaffEntity>());

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。StaffCode", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "", LoginId = "User01",  Name = "職員Ａ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" }
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_職員コード_ログインID登録済で2件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        var staffEntities = new List<StaffEntity> {
                                new StaffEntity(){ StaffCode = "S100", LoginId = "User01", Name = "職員Ａ",
                                                PasswordHash = new byte[] { 0 }, PasswordSalt = new byte[] { 0 },
                                                Enabled = true, RoleId  = (int)Ryobi.Wellship.Core.Enums.Role.User }
                            };
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User01" } )).ReturnsAsync(staffEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。StaffCode", InputNote  = "UT2011"},
            new(){Code = "10002", Message = "指定されたLoginIdが既に登録済みです。Code:User01", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "", LoginId = "User01",  Name = "職員Ａ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" }
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_ログインIDで2件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "" } )).ReturnsAsync(new List<StaffEntity>());

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。LoginId", InputNote  = "UT2011"},
            new(){Code = "10006", Message = "値の形式が無効です。LoginId:", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "S001", LoginId = "",  Name = "職員Ａ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" }
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_パスワードで1件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User01" } )).ReturnsAsync(new List<StaffEntity>{});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Password", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "S001", LoginId = "User01",  Name = "職員Ａ", Password = "",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" }
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 空値のチェック_職員名で1件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User01" } )).ReturnsAsync(new List<StaffEntity>{});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10004", Message = "必須項目が不足しています。Name", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "S001", LoginId = "User01",  Name = "", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" }
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 文字数のチェック_ログインIDで1件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User123456789012345678790" } )).ReturnsAsync(new List<StaffEntity>{});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10005", Message = "制限数を超えています。LoginId:User123456789012345678790", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "S001", LoginId = "User123456789012345678790",  Name = "職員Ａ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" }
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 文字数のチェック_パスワードで1件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User01" } )).ReturnsAsync(new List<StaffEntity>{});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10005", Message = "制限数を超えています。Password:P@ssw0rd123456789012345", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "S001", LoginId = "User01",  Name = "職員Ａ", Password = "P@ssw0rd123456789012345",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" }
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 文字制限のチェック_ログインIDで1件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User-001" } )).ReturnsAsync(new List<StaffEntity>{});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10006", Message = "値の形式が無効です。LoginId:User-001", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "S001", LoginId = "User-001",  Name = "職員Ａ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" }
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 重複キーのチェック_職員コードで2件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User01", "User02" } )).ReturnsAsync(new List<StaffEntity>{});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。StaffCode:S001", InputNote  = "UT2011"},
            new(){Code = "10003", Message = "キー項目が重複しています。StaffCode:S001", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "S001", LoginId = "User01",  Name = "職員Ａ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" },
            new Staff { StaffCode = "S001", LoginId = "User02",  Name = "職員Ａ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" },
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 重複キーのチェック_ログインIDで2件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User01" } )).ReturnsAsync(new List<StaffEntity>{});

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10003", Message = "キー項目が重複しています。LoginId:User01", InputNote  = "UT2011"},
            new(){Code = "10003", Message = "キー項目が重複しています。LoginId:User01", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "S001", LoginId = "User01",  Name = "職員Ａ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" },
            new Staff { StaffCode = "S002", LoginId = "User01",  Name = "職員Ｂ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" },
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task データのチェック_ログインIDが異なる職員コードで登録されているで1件のエラーが返る()
    {
        // Arrange
        // 職員コード、ログインIDに紐づく職員情報を取得する
        var staffEntities = new List<StaffEntity> {
                                new StaffEntity(){ StaffCode = "S100", LoginId = "User01", Name = "職員Ａ",
                                                PasswordHash = new byte[] { 0 }, PasswordSalt = new byte[] { 0 },
                                                Enabled = true, RoleId  = (int)Ryobi.Wellship.Core.Enums.Role.User }
                            };
        _staffRepositoryMock.Setup(x => x.GetStaffsInfoByLoginIdsAsync(new List<string> { "User01" } )).ReturnsAsync(staffEntities);

        // WARNING検証エラー
        var expected = new List<ErrorObject>() {
            new(){Code = "10002", Message = "指定されたLoginIdが既に登録済みです。Code:User01", InputNote  = "UT2011"}
        };
        var usecase = new StaffUsecase(_staffRepositoryMock.Object, _timeProvider);
        var request = new List<Staff>
        {
            new Staff { StaffCode = "S001", LoginId = "User01",  Name = "職員Ａ", Password = "P@ssw0rd",
                        Enabled = true, RoleId = Ryobi.Wellship.Core.Enums.Role.User, InputNote = "UT2011" }
        };
        // Act
        var errors = await usecase.StoreStaffsAsync(request);
        // Assert
        errors.Should().BeEquivalentTo(expected);
    }
}
