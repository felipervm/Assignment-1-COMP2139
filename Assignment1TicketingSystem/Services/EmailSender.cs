using MailKit.Net.Smtp;
using MimeKit;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    public EmailSender(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["Smtp:From"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlMessage };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]), false);
        if (!string.IsNullOrEmpty(_config["Smtp:User"]))
            await smtp.AuthenticateAsync(_config["Smtp:User"], _config["Smtp:Pass"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
