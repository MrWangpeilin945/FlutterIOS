namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 検査メニュー特記
/// 検査結果値や受診に紐づく特記事項をセットして表示用文字列を組み立てるクラス
/// </summary>
public class MenuNoteResult
{

    private readonly MenuNote _menuNote;
    private readonly IEnumerable<ExamItemDetailChild> _detailChildren;
    private readonly IEnumerable<ExamItemDetailResult> _currentResults;
    private readonly IEnumerable<ExamItemDetailResult> _previousResults;
    private readonly IEnumerable<ConsultNote> _consultNote;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="menuNote">検査メニュー特記のマスタ</param>
    /// <param name="detailChildren">検査項目明細とその子要素のマスタ</param>
    /// <param name="examResult">今回値</param>
    /// <param name="previousResult">前回値</param>
    /// <param name="consultNotes">受診特記</param>
    public MenuNoteResult(MenuNote menuNote, IEnumerable<ExamItemDetailChild> detailChildren, ExamResult examResult, PreviousResult previousResult, IEnumerable<ConsultNote> consultNotes)
    {
        _menuNote = menuNote;
        _detailChildren = detailChildren;
        _currentResults = examResult.ExamItemDetailResults;
        _previousResults = previousResult.ExamItemDetailResults;
        _consultNote = consultNotes;
    }

    /// <summary>
    /// 特記の表示用に文字列を組み立てて取得します。
    /// </summary>
    public string GetDisplayText()
    {
        // 親テーブルに
        if (_menuNote.ExamResults.Any())
        {
            return GetDisplayResultText();
        }

        if (_menuNote.ConsultNotes.Any())
        {
            return GetDisplayConsultNoteText();
        }

        return "";
    }

    private string GetDisplayResultText()
    {
        var 今回値の明細IDs = _menuNote.ExamResults.Where(x => x.SourceType == Core.Enums.SourceType.今回値).Select(x => x.ExamItemDetailId).ToArray();
        var 前回値の明細IDs = _menuNote.ExamResults.Where(x => x.SourceType == Core.Enums.SourceType.前回値).Select(x => x.ExamItemDetailId).ToArray();

        var 表示用今回値リスト = 今回値の明細IDs.Select(x => Get表示用値(x, _currentResults));
        var 表示用前回値リスト = 前回値の明細IDs.Select(x => Get表示用値(x, _previousResults));

        var 今回値テキスト = string.Join("/", 表示用今回値リスト);
        var 前回値テキスト = string.Join("/", 表示用前回値リスト);

        return $"{今回値テキスト}({前回値テキスト}){_menuNote.Suffix}";
    }

    private string GetDisplayConsultNoteText()
    {
        var consultNotes = _menuNote.ConsultNotes.Select(x => _consultNote.SingleOrDefault(c => c.Code == x.Code)).Select(x => x?.Note ?? "");
        var joinText = string.Join("/", consultNotes);
        return $"{joinText}{_menuNote.Suffix}";
    }

    private string Get表示用値(int 明細ID, IEnumerable<ExamItemDetailResult> 回答リスト)
    {
        var マスタ = _detailChildren.SingleOrDefault(x => x.ExamItemDetailId == 明細ID);

        if (マスタ is null)
        {
            return "";
        }

        var 回答値 = 回答リスト.SingleOrDefault(x => x.ExamItemDetailId == 明細ID)?.Value ?? "";

        return マスタ.Type switch
        {
            Core.Enums.ExamItemDetailType.入力 => 回答値,
            Core.Enums.ExamItemDetailType.選択 => マスタ.DetailOptions.SingleOrDefault(x => x.Code == 回答値)?.Name ?? "",
            _ => ""
        };
    }
}
