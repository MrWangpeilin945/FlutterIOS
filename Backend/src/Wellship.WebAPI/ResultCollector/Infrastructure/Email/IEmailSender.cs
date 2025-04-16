namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Email;

/// <summary>
/// メール送信のインターフェース
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// 複数件のメールを一括送信します。
    /// </summary>
    public Task SendBulkEmailAsync(IEnumerable<Domain.Models.Email> emails);
}