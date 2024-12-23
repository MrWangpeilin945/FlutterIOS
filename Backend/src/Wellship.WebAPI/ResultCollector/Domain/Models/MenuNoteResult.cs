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
    /// 検査メニュー特記の名称
    /// 画面上にタグとして表示する
    /// </summary>
    public string MenuNoteName
    {
        get
        {
            return _menuNote.Name;
        }
    }

    /// <summary>
    /// 特記の表示用に文字列を組み立てて取得します。
    /// </summary>
    public string GetDisplayText()
    {
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
    {   // 今回値の明細IDs
        var currentDetailIds = _menuNote.ExamResults.Where(x => x.SourceType == Core.Enums.SourceType.今回値).Select(x => x.ExamItemDetailId).ToArray();
        // 前回値の明細IDs
        var previousDetailIds = _menuNote.ExamResults.Where(x => x.SourceType == Core.Enums.SourceType.前回値).Select(x => x.ExamItemDetailId).ToArray();

        // 表示用今回値リスト
        var displayCurrentList = currentDetailIds.Select(x => GetDisplayValue(x, _currentResults));
        // 表示用前回値リスト
        var displatPreviousList = previousDetailIds.Select(x => GetDisplayValue(x, _previousResults));

        // 今回値テキスト
        var currentText = string.Join("/", displayCurrentList);
        // 前回値テキスト
        var previousText = string.Join("/", displatPreviousList);

        return $"{currentText}({previousText}){_menuNote.Suffix}";
    }

    private string GetDisplayConsultNoteText()
    {
        var consultNotes = _menuNote.ConsultNotes.Select(x => _consultNote.SingleOrDefault(c => c.Code == x.Code))
                                                 .Select(x => x?.Note ?? "");
        var joinText = string.Join("/", consultNotes);
        return $"{joinText}{_menuNote.Suffix}";
    }

    private string GetDisplayValue(int detailId, IEnumerable<ExamItemDetailResult> answerList)
    {
        var master = _detailChildren.SingleOrDefault(x => x.ExamItemDetailId == detailId);

        if (master is null)
        {
            return "";
        }

        var answerValue = answerList.SingleOrDefault(x => x.ExamItemDetailId == detailId)?.Value ?? "";

        return master.Type switch
        {
            Core.Enums.ExamItemDetailType.入力 => answerValue,
            Core.Enums.ExamItemDetailType.選択 => master.DetailOptions.SingleOrDefault(x => x.Code == answerValue)?.Name ?? answerValue,
            _ => ""
        };
    }
}
