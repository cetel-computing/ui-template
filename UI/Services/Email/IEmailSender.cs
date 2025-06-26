using System.Threading.Tasks;

namespace FlazorTemplate.Services.Email
{
    /// <summary>
    /// Service to send emails.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="emailTo">The email address to send the email to.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="message">The body of the email.</param>
        /// <param name="emailFrom">The optional email address to send the email from.</param>
        /// <returns>A task representing the sending of the email.</returns>
        Task SendEmailAsync(string emailTo, string subject, string message, string? emailFrom = null);
    }
}
