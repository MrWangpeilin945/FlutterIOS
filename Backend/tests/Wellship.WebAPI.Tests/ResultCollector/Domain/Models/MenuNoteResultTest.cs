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

    [Fact]
    public void 検査メニュー特記のサブタイプのマスタがない_空文字()
    {
        // Arrange

        var menuNote = new MenuNote()
        {
            MenuNoteId = 1,
            Name = "検査メニュー特記テスト",
            ExamMenuId = 1,
            Suffix = "番",
            ConsultNotes = [],
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

        List<ConsultNote> consultNotes = [];

        var menuNoteResult = new MenuNoteResult(menuNote, detailChildren, examResult, previousResult, consultNotes);


        // Act
        var actual = menuNoteResult.GetDisplayText();

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void 未知の検査項目明細ID_空文字()
    {
        // Arrange

        var menuNote = new MenuNote()
        {
            MenuNoteId = 1,
            Name = "検査メニュー特記テスト",
            ExamMenuId = 1,
            Suffix = "",
            ConsultNotes = [],
            ExamResults = [
                new() { ExamItemDetailId = 12, SourceType = Core.Enums.SourceType.今回値},
                new() { ExamItemDetailId = 12, SourceType = Core.Enums.SourceType.前回値}
            ]
        };

        // ここが取得できない場合のテスト
        List<ExamItemDetailChild> detailChildren = [];

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

        List<ConsultNote> consultNotes = [];

        var menuNoteResult = new MenuNoteResult(menuNote, detailChildren, examResult, previousResult, consultNotes);
        var expected = "()";

        // Act
        var actual = menuNoteResult.GetDisplayText();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void 選択肢の場合はコードから名称に変換する_名称が引ける場合は名称()
    {
        // Arrange

        var menuNote = new MenuNote()
        {
            MenuNoteId = 1,
            Name = "選択肢の名称変換テスト",
            ExamMenuId = 1,
            Suffix = "",
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
                Name = "選択肢系のの明細",
                PositionNumber = 1,
                EquipmentLabel = "",
                Unit = "cm",
                Type = Core.Enums.ExamItemDetailType.選択,
                KeyboardType = Core.Enums.KeyboardType.テンキー,
                Keyboards = [],
                DetailOptions = [
                    new ExamItemDetailOption(){ExamItemDetailId = 12, Code = "101", Name = "選択肢A", OrderNumber = 1},
                    new ExamItemDetailOption(){ExamItemDetailId = 12, Code = "102", Name = "選択肢B", OrderNumber = 2},
                    new ExamItemDetailOption(){ExamItemDetailId = 12, Code = "103", Name = "選択肢C", OrderNumber = 3},
                ]
            }
        ];

        ExamResult examResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamItemDetailResults = [
                new(){ ExamItemId = 1, ExamItemDetailId = 12, Value = "103" }
            ]
        };

        PreviousResult previousResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamDate = new DateOnly(2024, 12, 30),
            ExamItemDetailResults = [
                new(){ ExamItemId = 1, ExamItemDetailId = 12, Value = "101" }
            ]
        };

        List<ConsultNote> consultNotes = [];

        var menuNoteResult = new MenuNoteResult(menuNote, detailChildren, examResult, previousResult, consultNotes);

        var expected = "選択肢C(選択肢A)";

        // Act
        var actual = menuNoteResult.GetDisplayText();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void 選択肢の場合はコードから名称に変換する_名称が引けない場合はコードのまま()
    {
        // Arrange

        var menuNote = new MenuNote()
        {
            MenuNoteId = 1,
            Name = "選択肢の名称変換テスト",
            ExamMenuId = 1,
            Suffix = "",
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
                Name = "選択肢系のの明細",
                PositionNumber = 1,
                EquipmentLabel = "",
                Unit = "cm",
                Type = Core.Enums.ExamItemDetailType.選択,
                KeyboardType = Core.Enums.KeyboardType.テンキー,
                Keyboards = [],
                DetailOptions = [
                    new ExamItemDetailOption(){ExamItemDetailId = 12, Code = "101", Name = "選択肢A", OrderNumber = 1},
                    new ExamItemDetailOption(){ExamItemDetailId = 12, Code = "102", Name = "選択肢B", OrderNumber = 2},
                    new ExamItemDetailOption(){ExamItemDetailId = 12, Code = "103", Name = "選択肢C", OrderNumber = 3},
                ]
            }
        ];

        ExamResult examResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamItemDetailResults = [
                new(){ ExamItemId = 1, ExamItemDetailId = 12, Value = "104" }
            ]
        };

        PreviousResult previousResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamDate = new DateOnly(2024, 12, 30),
            ExamItemDetailResults = [
                new(){ ExamItemId = 1, ExamItemDetailId = 12, Value = "101" }
            ]
        };

        List<ConsultNote> consultNotes = [];

        var menuNoteResult = new MenuNoteResult(menuNote, detailChildren, examResult, previousResult, consultNotes);

        var expected = "104(選択肢A)";

        // Act
        var actual = menuNoteResult.GetDisplayText();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void 検査項目明細種別が入力と選択以外の場合は空文字()
    {
        // Arrange

        var menuNote = new MenuNote()
        {
            MenuNoteId = 1,
            Name = "入力と選択以外",
            ExamMenuId = 1,
            Suffix = "",
            ConsultNotes = [],
            ExamResults = [
                new() { ExamItemDetailId = 12, SourceType = Core.Enums.SourceType.今回値}
            ]
        };

        List<ExamItemDetailChild> detailChildren = [
            new(){
                ExamItemDetailId = 12,
                IntegerLength = 3,
                DecimalLength = 1,
                Name = "演算系の明細",
                PositionNumber = 1,
                EquipmentLabel = "",
                Unit = "cm",
                Type = Core.Enums.ExamItemDetailType.演算値,
                KeyboardType = Core.Enums.KeyboardType.テンキー,
                Keyboards = [],
                DetailOptions = []
            }
        ];

        ExamResult examResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamItemDetailResults = [
                new(){ ExamItemId = 1, ExamItemDetailId = 12, Value = "999" }
            ]
        };

        PreviousResult previousResult = new()
        {
            ConsultId = Guid.Parse("823f1cfd-39d6-4e41-b2fa-38027009a4d1"),
            ExamDate = new DateOnly(2024, 12, 30),
            ExamItemDetailResults = []
        };

        List<ConsultNote> consultNotes = [];

        var menuNoteResult = new MenuNoteResult(menuNote, detailChildren, examResult, previousResult, consultNotes);
        var expected = "()";

        // Act
        var actual = menuNoteResult.GetDisplayText();

        // Assert
        actual.Should().Be(expected);
    }
}
