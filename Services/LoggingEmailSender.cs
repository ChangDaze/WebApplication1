using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    // Development email sender: writes the email to the console/log instead of
    // actually sending it. Swap for a real SMTP/SendGrid sender in production.
    public class LoggingEmailSender : IEmailSender
    {
        private readonly ILogger<LoggingEmailSender> _logger;

        public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation(
                "\n===== EMAIL (dev) =====\nTo: {To}\nSubject: {Subject}\n\n{Body}\n=======================",
                to, subject, body);

            return Task.CompletedTask;
        }
    }
}
