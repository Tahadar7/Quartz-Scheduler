namespace QuartzScheduler.API.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toAddress, string subject, string body, bool isHtml, CancellationToken ct = default);
}