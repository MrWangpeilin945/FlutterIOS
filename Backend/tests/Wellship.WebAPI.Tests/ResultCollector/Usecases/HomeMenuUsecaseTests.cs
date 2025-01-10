using FluentAssertions;

using Moq;

using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class HomemenuUsecaseTests
{
    private readonly Mock<IHomeMenuRepository> _homeMenuRepositoryMock;
    private readonly Mock<IPlaceScheduleRepository> _placeScheduleRepositoryMock;
    private readonly IEnumerable<WebAPI.ResultCollector.Domain.Models.HomeMenuGroup> _homeMenuGroups;
    private readonly WebAPI.ResultCollector.Domain.Models.PlaceScheduleStatus _placeScheduleStatus;

    public HomemenuUsecaseTests()
    {
        _placeScheduleRepositoryMock = new Mock<IPlaceScheduleRepository>();
        _homeMenuRepositoryMock = new Mock<IHomeMenuRepository>();

        _homeMenuGroups = [
            new(){
                GroupId = 1,
                GroupName = "一般",
                HomeMenus = [
                    new(){
                        MenuId = 1,
                        MenuName = "検査メニュー選択",
                        Path = "exammenu-select"
                    },
                    new(){
                        MenuId = 2,
                        MenuName = "進捗",
                        Path = "progress"
                    }
                ]
            },
            new(){
                GroupId = 2,
                GroupName = "管理",
                HomeMenus = [
                    new(){
                        MenuId = 3,
                        MenuName = "会場ロック",
                        Path = "placeschedule-lock"
                    },
                    new(){
                        MenuId = 4,
                        MenuName = "検査結果出力",
                        Path = "examresult-export"
                    },
                    new(){
                        MenuId = 5,
                        MenuName = "検査結果出力履歴",
                        Path = "examresult-export-history"
                    }
                ]
            }
        ];

        _placeScheduleStatus = new WebAPI.ResultCollector.Domain.Models.PlaceScheduleStatus()
        {
            PlaceScheduleId = Guid.Parse("75f3d492-4a3e-477d-b67b-c4320ce77dba"),
            PlaceId = Guid.Parse("d74b6117-e784-4607-9dc4-5218b07e23d6"),
            PlaceName = "会場A",
            ExamDate = new DateTime(2024, 12, 20),
            Status = PlaceScheduleLockingStatus.検査中,
            CreatedAt = new DateTime(2024, 12, 10),
            CreatedBy = "登録者A"
        };
    }

    [Fact]
    public async Task 一般ロールでメニュー一覧を取得する_会場日程IDあり()
    {
        // Arrange
        var placeScheduleId = Guid.Parse("75f3d492-4a3e-477d-b67b-c4320ce77dba");

        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleLockingStatusAsync(It.IsAny<Guid>())).ReturnsAsync(_placeScheduleStatus);
        _homeMenuRepositoryMock.Setup(x => x.GetHomeMenuGroupsAsync()).ReturnsAsync(_homeMenuGroups);

        var expected = new HomeMenuGroupList()
        {
            PlaceScheduleLockingStatus = (int)PlaceScheduleLockingStatus.検査中,
            StaffRole = (int)Role.User,
            HomeMenuGroups = [
                       new HomeMenuGroup(){
                        GroupName = "一般",
                        Menus = [
                            new(){
                                MenuName = "検査メニュー選択",
                                Path = "exammenu-select",
                                AvailableConditions = ["PlaceScheduleSelected", "PlaceScheduleUnlocked"]
                            },
                            new(){
                                MenuName = "進捗",
                                Path = "progress",
                                AvailableConditions = ["PlaceScheduleSelected"]
                            }
                        ]
                        }
            ]
        };

        var homeMenuUsecase = new HomeMenuUsecase(_homeMenuRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

        // Act
        var result = await homeMenuUsecase.GetHomeMenusAsync(placeScheduleId);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 一般ロールでメニュー一覧を取得する_会場日程IDなし()
    {
        // Arrange
        Guid? placeScheduleId = null;

        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleLockingStatusAsync(It.IsAny<Guid>())).ReturnsAsync(_placeScheduleStatus);
        _homeMenuRepositoryMock.Setup(x => x.GetHomeMenuGroupsAsync()).ReturnsAsync(_homeMenuGroups);

        var expected = new HomeMenuGroupList()
        {
            PlaceScheduleLockingStatus = null,
            StaffRole = (int)Role.User,
            HomeMenuGroups = [
                       new HomeMenuGroup(){
                        GroupName = "一般",
                        Menus = [
                            new(){
                                MenuName = "検査メニュー選択",
                                Path = "exammenu-select",
                                AvailableConditions = ["PlaceScheduleSelected", "PlaceScheduleUnlocked"]
                            },
                            new(){
                                MenuName = "進捗",
                                Path = "progress",
                                AvailableConditions = ["PlaceScheduleSelected"]
                            }
                        ]
                        }
            ]
        };

        var homeMenuUsecase = new HomeMenuUsecase(_homeMenuRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

        // Act
        var result = await homeMenuUsecase.GetHomeMenusAsync(placeScheduleId);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact(Skip = "TODO: プロダクトコードのロール取得実装待ち")]

    public async Task 管理者ロールでメニュー一覧を取得する_会場日程IDあり()
    {
        // Arrange
        var placeScheduleId = Guid.Parse("75f3d492-4a3e-477d-b67b-c4320ce77dba");

        _placeScheduleRepositoryMock.Setup(x => x.GetPlaceScheduleLockingStatusAsync(It.IsAny<Guid>())).ReturnsAsync(_placeScheduleStatus);
        _homeMenuRepositoryMock.Setup(x => x.GetHomeMenuGroupsAsync()).ReturnsAsync(_homeMenuGroups);

        var expected = new HomeMenuGroupList()
        {
            PlaceScheduleLockingStatus = (int)PlaceScheduleLockingStatus.検査中,
            StaffRole = (int)Role.Admin,
            HomeMenuGroups = [
                       new HomeMenuGroup(){
                        GroupName = "一般",
                        Menus = [
                            new(){
                                MenuName = "検査メニュー選択",
                                Path = "exammenu-select",
                                AvailableConditions = ["PlaceScheduleSelected", "PlaceScheduleUnlocked"]
                            },
                            new(){
                                MenuName = "進捗",
                                Path = "progress",
                                AvailableConditions = ["PlaceScheduleSelected"]
                            }
                        ]
                        },
                        new HomeMenuGroup(){
                        GroupName = "管理",
                        Menus = [
                            new(){
                                MenuName = "会場ロック",
                                Path = "placeschedule-lock",
                                AvailableConditions = ["PlaceScheduleSelected", "PlaceScheduleUnlocked"]
                            },
                            new(){
                                MenuName = "検査結果出力",
                                Path = "examresult-export",
                                AvailableConditions = []
                            },
                            new(){
                                MenuName = "検査結果出力履歴",
                                Path = "examresult-export-history",
                                AvailableConditions = []
                            }
                        ]
                        }
            ]
        };

        var homeMenuUsecase = new HomeMenuUsecase(_homeMenuRepositoryMock.Object, _placeScheduleRepositoryMock.Object);

        // Act
        var result = await homeMenuUsecase.GetHomeMenusAsync(placeScheduleId);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
