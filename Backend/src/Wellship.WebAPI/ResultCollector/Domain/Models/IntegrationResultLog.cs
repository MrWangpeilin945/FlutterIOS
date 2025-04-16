using System.Text;

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
            const int maxDetailsPerGroup = 1000;

            // DetailsをFunctionCodeでグループ化
            var groupedDetails = Details.GroupBy(d => d.FunctionCode);

            // メール表示用のテキストを組み立てる
            var emailText = new StringBuilder();
            foreach (var group in groupedDetails)
            {
                emailText.AppendLine($"詳細機能コード: {group.Key}");
                // 詳細機能コードでグループ化して、グループあたりの明細件数を制限する
                // 無制限だとメールの容量的に送信できない恐れがあるため
                var limitedDetails = group.Take(maxDetailsPerGroup);
                foreach (var detail in limitedDetails)
                {
                    var propertiesText = string.Join("/", detail.Properties.Select(p => p.Name));
                    emailText.AppendLine($"{detail.EventSource} {detail.ResultDetailMessage} {propertiesText}");
                }
                emailText.AppendLine();
            }
            return emailText.ToString().TrimEnd(); ;
        }
    }
}
