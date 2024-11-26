using FluentAssertions;

using Moq;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;
using Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Usecases;

public class EquipmentUsecaseTests
{
    [Fact]
    public async Task 検査機器一覧を取得する_複数()
    {
        // Arrange
        var examMenuId = 1;
        var equipmentRepositoryMock = new Mock<IEquipmentRepository>();

        var equipments = new List<Equipment>() {
            new(){EquipmentId = 1, EquipmentName = "EQ001", AppLaunchUrl = "wsc://aaaa", ProcessingScriptUrl  = "https://example.com/scripts/eq001.js"},
            new(){EquipmentId = 2, EquipmentName = "EQ002", AppLaunchUrl = "wsc://aaaa", ProcessingScriptUrl  = "https://example.com/scripts/eq002.js"},
            new(){EquipmentId = 3, EquipmentName = "EQ003", AppLaunchUrl = "wsc://aaaa", ProcessingScriptUrl  = "https://example.com/scripts/eq003.js"},
         };

        equipmentRepositoryMock.Setup(r => r.GetEquipmentAsync(examMenuId))
                               .ReturnsAsync(equipments);
        var equipmentUsecase = new EquipmentUsecase(equipmentRepositoryMock.Object);

        var expected = new APIModels.Responses.EquipmentList()
        {
            Equipments = [
                new(){EquipmentId = 1, EquipmentName = "EQ001", AppLaunchUrl = "wsc://aaaa", ProcessingScriptUrl  = "https://example.com/scripts/eq001.js"},
                new(){EquipmentId = 2, EquipmentName = "EQ002", AppLaunchUrl = "wsc://aaaa", ProcessingScriptUrl  = "https://example.com/scripts/eq002.js"},
                new(){EquipmentId = 3, EquipmentName = "EQ003", AppLaunchUrl = "wsc://aaaa", ProcessingScriptUrl  = "https://example.com/scripts/eq003.js"},
            ]
        };

        // Act
        var result = await equipmentUsecase.GetEquipmentsAsync(examMenuId);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 検査機器一覧を取得する_0件()
    {
        // Arrange
        var examMenuId = 1;
        var equipmentRepositoryMock = new Mock<IEquipmentRepository>();

        var equipments = new List<Equipment>();

        equipmentRepositoryMock.Setup(r => r.GetEquipmentAsync(examMenuId));
        var equipmentUsecase = new EquipmentUsecase(equipmentRepositoryMock.Object);

        var expected = new APIModels.Responses.EquipmentList()
        {
            Equipments = []
        };

        // Act
        var result = await equipmentUsecase.GetEquipmentsAsync(examMenuId);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
