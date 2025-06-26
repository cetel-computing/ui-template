namespace FlazorTemplate.Store.Features.Modals.Actions
{
    public class SaveFeedback
    {
        public SaveFeedback(string email, string feedback)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException($"'{nameof(email)}' cannot be null or whitespace", nameof(email));
            }
            if (string.IsNullOrWhiteSpace(feedback))
            {
                throw new ArgumentException($"'{nameof(feedback)}' cannot be null or whitespace", nameof(feedback));
            }

            FromEmail = email;
            Feedback = feedback;
        }

        public string FromEmail { get; }

        public string Feedback { get; }        
    }
}
