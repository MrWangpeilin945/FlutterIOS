using Amazon.SimpleEmail.Model;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Email.AmazonSES;

/// <summary>
/// Eメールの拡張
/// </summary>
public static class EmailExtensions
{
    /// <summary>
    /// EメールのドメインモデルをSESのリクエストモデルに変換します。
    /// </summary>
    /// <param name="email">Eメールのドメインモデル</param>
    /// <returns>SESのリクエストモデル</returns>
    public static SendEmailRequest ToSendEmailRequest(this Domain.Models.Email email)
    {
        return new SendEmailRequest
        {
            Source = email.Sender,
            Destination = new Destination
            {
                ToAddresses = email.Recipients
            },
            Message = new Message
            {
                Subject = new Content(email.Subject),
                Body = new Body
                {
                    Html = new Content(email.Body)
                }
            }
        };
    }
}
