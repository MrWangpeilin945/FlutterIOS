using FluentAssertions;

using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

namespace Ryobi.Wellship.WebAPI.Tests.ResultCollector.Domain.Models;
public class MenuNoteResultTest
{
    [Fact]
    public void 前回値と今回値_各1つ()
    {
        // Arrange

        var menuNote = new MenuNote()
        {
            MenuNoteId = 1,
            Name = "身長",
            ExamMenuId = 1,
            Suffix = "cm",
            ConsultNotes = [],
            ExamResults = [
                new() { ExamItemDetailId = 12, SourceType = Core.Enums.SourceType.今回値},
                new() { ExamItemDetailId = 12, SourceType = Core.Enums.SourceType.前回値}
            ]
        };

        List<ExamItemDetailChild> detailChildren = [
            new(){
                ExamItemDetailId = 12,
                IntegerLength = 3,
                DecimalLength = 1,
                Name = "身長",
                PositionNumber = 1,
                EquipmentLabel = "",
                Unit = "cm",
                Type = Core.Enums.ExamItemDetailType.入力,
                KeyboardType = Core.Enums.KeyboardType.テンキー,
                Keyboards = [],
                DetailOptions = []
            }
        ];

        ExamResult examResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamItemDetailResults = [
                new(){ ExamItemId = 1, ExamItemDetailId = 12, Value = "175.3" }
            ]
        };

        PreviousResult previousResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamDate = new DateOnly(2024, 12, 30),
            ExamItemDetailResults = [
                new(){ ExamItemId = 1, ExamItemDetailId = 12, Value = "173.0" }
            ]
        };

        List<ConsultNote> consultNotes = [
            new ConsultNote(){Code = "012",Note = "撮影番号"}
        ];

        var menuNoteResult = new MenuNoteResult(menuNote, detailChildren, examResult, previousResult, consultNotes);

        var expected = "175.3(173.0)cm";

        // Act
        var actual = menuNoteResult.GetDisplayText();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void 受診特記_2つ()
    {
        // Arrange

        var menuNote = new MenuNote()
        {
            MenuNoteId = 1,
            Name = "撮影番号/○○番号",
            ExamMenuId = 1,
            Suffix = "番",
            ConsultNotes = [
                new() { Code = "ST01" },
                new() { Code = "ST02" }
            ],
            ExamResults = []
        };

        List<ExamItemDetailChild> detailChildren = [];

        ExamResult examResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamItemDetailResults = []
        };

        PreviousResult previousResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamDate = new DateOnly(2024, 12, 30),
            ExamItemDetailResults = []
        };

        List<ConsultNote> consultNotes = [
            new ConsultNote(){Code = "ST01",Note = "01-001"},
            new ConsultNote(){Code = "ST02",Note = "02-001"}
        ];

        var menuNoteResult = new MenuNoteResult(menuNote, detailChildren, examResult, previousResult, consultNotes);

        var expected = "01-001/02-001番";

        // Act
        var actual = menuNoteResult.GetDisplayText();

        // Assert
        actual.Should().Be(expected);
    }
}
