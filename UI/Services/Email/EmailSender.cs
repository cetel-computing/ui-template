using System;
using System.Threading.Tasks;
using FluentEmail.Core;

namespace FlazorTemplate.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailSender(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail ?? throw new ArgumentNullException(nameof(fluentEmail));
        }

        public Task SendEmailAsync(string emailTo, string subject, string message, string? emailFrom = null)
        {
            return _fluentEmail
                .SetFrom(emailFrom)
                .To(emailTo)
                .Subject(subject)
                .Body(message, true)
            .SendAsync();
        }
    }
}
