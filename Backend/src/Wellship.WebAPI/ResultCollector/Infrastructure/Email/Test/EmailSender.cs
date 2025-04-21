namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Email.Test;

/// <summary>
/// メール送信のテスト用
/// </summary>
public sealed class EmailSender : IEmailSender
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EmailSender()
    {

    }

    /// <inheritdoc/>
    public Task SendBulkEmailAsync(IEnumerable<Domain.Models.Email> emails)
    {
        foreach (var email in emails)
        {
            Console.WriteLine("Sending Email:");
            Console.WriteLine($"From: {email.Sender}");
            Console.WriteLine($"To: {string.Join(", ", email.Recipients)}");
            Console.WriteLine($"Subject: {email.Subject}");
            Console.WriteLine("Body:");
            Console.WriteLine(email.Body);
            Console.WriteLine("--------------------------------------------------");
        }

        return Task.CompletedTask;
    }
}