namespace FlazorTemplate.Configuration
{
    public record MailConfig
    {
        public string MailServer { get; init; }

        public int MailPort { get; init; } = 443;

        public string SenderName { get; init; } = "";

        public string Sender { get; init; } = "";

        public bool EnableSsl { get; init; } = true;

        /// <summary>
        /// The email address that Feedback should be sent to.
        /// </summary>
        public string FeedbackAddress { get; init; }
    }
}
