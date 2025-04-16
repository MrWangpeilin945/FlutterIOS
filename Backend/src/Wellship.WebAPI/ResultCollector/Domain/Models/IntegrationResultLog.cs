namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models;

/// <summary>
/// 連携処理結果ログ
/// </summary>
public sealed class IntegrationResultLog
{
    /// <summary>
    /// ログID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// 概要
    /// </summary>
    public required string Summary { get; init; }

    /// <summary>
    /// ログレベル
    /// </summary>
    public required LogLevel LogLevel { get; init; }

    /// <summary>
    /// 処理結果コード
    /// </summary>
    public required string ResultCode { get; init; }

    /// <summary>
    /// 処理結果コード名称
    /// </summary>
    public required string ResultCodeName { get; init; }

    /// <summary>
    /// 機能コード
    /// </summary>
    public required string FunctionCode { get; init; }

    /// <summary>
    /// 機能名
    /// </summary>
    public required string FunctionName { get; init; }

    /// <summary>
    /// 発生日時
    /// </summary>
    public required DateTimeOffset OccurredAt { get; init; }

    /// <summary>
    /// ログ明細
    /// </summary>
    public required List<IntegrationResultLogDetail> Details { get; init; }

    /// <summary>
    /// メール表示用の明細テキスト
    /// </summary>
    public string EmailDetailsText
    {
        get
        {
            // TODO: 明細からメール表示用のテキストを組み立てる
            // Details.GroupBy(d => d.FunctionCode)
            return @"詳細機能ID: EC2009
基準値(範囲)_20250324.csv Row:2 指定されたコードがマスタに登録されていません。ExamItemDetailCd

詳細機能ID: EC2004
予約情報_20250324.csv Row:4 指定されたコードがマスタに登録されていません。TeamCode
予約情報_20250324.csv Row:2 指定されたコードがマスタに登録されていません。PlaceCode/TeamCode/ExamDate
予約情報_20250324.csv Row:5 指定されたコードがマスタに登録されていません。ExamineeCd

詳細機能ID: EC2002
受付情報_20250324.csv Row:3 値が登録されていません。TicketNumber
受付情報_20250324.csv Row:2 指定されたコードがマスタに登録されていません。ConnectionCode
";
        }
    }
}
