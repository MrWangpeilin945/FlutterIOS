using Amazon;
using Amazon.SimpleEmail;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Email.AmazonSES;

/// <summary>
/// AmazonSESを使用したメール送信
/// </summary>
public sealed class EmailSender : IEmailSender
{
    private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EmailSender()
    {
        _amazonSimpleEmailService = new AmazonSimpleEmailServiceClient(RegionEndpoint.APNortheast1);
    }

    /// <inheritdoc/>
    public async Task SendBulkEmailAsync(IEnumerable<Domain.Models.Email> emails)
    {
        foreach (var email in emails)
        {
            var sendRequest = email.ToSendEmailRequest();

            try
            {
                var response = await _amazonSimpleEmailService.SendEmailAsync(sendRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
