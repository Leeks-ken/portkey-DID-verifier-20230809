using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;

namespace CAVerifierServer.Email;

public class AwsEmailSender : EmailSenderBase
{
    private readonly AwsEmailOptions _awsEmailOptions;
    private readonly ILogger<AwsEmailSender> _logger;


    public AwsEmailSender(IOptions<AwsEmailOptions> awsEmailOptions, ILogger<AwsEmailSender> logger,
        IEmailSenderConfiguration configuration, IBackgroundJobManager backgroundJobManager) : base(configuration,
        backgroundJobManager)
    {
        _logger = logger;
        _awsEmailOptions = awsEmailOptions.Value;
    }

    public override async Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        await SendAsync(_awsEmailOptions.From, to, subject, body, isBodyHtml);
    }

    public override async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
    {
        var mail = new MailMessage();
        mail.IsBodyHtml = true;
        mail.From = new MailAddress(from, _awsEmailOptions.FromName);
        mail.To.Add(new MailAddress(to));
        mail.Subject = subject;
        mail.Body = body;
        // const string configSetHeaderName = "X-SES-CONFIGURATION-SET";
        // mail.Headers.Add(configSetHeaderName, _awsEmailOption.ConfigSet);
        await SendEmailAsync(mail);
    }

    public async Task SendAsync(MailMessage mail)
    {
        using var client = new SmtpClient(_awsEmailOptions.Host, _awsEmailOptions.Port);
        // Pass SMTP credentials
        client.Credentials =
            new NetworkCredential(_awsEmailOptions.SmtpUsername, _awsEmailOptions.SmtpPassword);

        // Enable SSL encryption
        client.EnableSsl = true;
        // Try to send the message. Show status in console.
        try
        {
            _logger.LogInformation($"Attempting to send email to {mail.To} via aws");
            client.Send(mail);
            _logger.LogInformation($"Email sent to {mail.To} via aws");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"send aws email failed, to={mail.To}");
            throw ex;
        }
    }

    protected override async Task SendEmailAsync(MailMessage mail)
    {
        using var client = new SmtpClient(_awsEmailOptions.Host, _awsEmailOptions.Port);
        // Pass SMTP credentials
        client.Credentials =
            new NetworkCredential(_awsEmailOptions.SmtpUsername, _awsEmailOptions.SmtpPassword);

        // Enable SSL encryption
        client.EnableSsl = true;
        // Try to send the message. Show status in console.
        try
        {
            _logger.LogInformation($"Attempting to send email to {mail.To} via aws");
            await client.SendMailAsync(mail);
            _logger.LogInformation($"Email sent to {mail.To} via aws");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"send aws email failed, to={mail.To}");
            throw ex;
        }
    }
}