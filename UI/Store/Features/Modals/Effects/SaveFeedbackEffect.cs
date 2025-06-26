using Microsoft.Extensions.Options;
using Fluxor;
using Framework.Flazor;
using Framework.Flazor.Token;
using FlazorTemplate.Configuration;
using FlazorTemplate.Services.Email;
using FlazorTemplate.Store.Features.Modals.Actions;

namespace FlazorTemplate.Store.Features.Modals.Effects
{
    public class SaveFeedbackEffect : BaseEffect<SaveFeedback>
    {
        private readonly IEmailSender _emailSender;
        private readonly IOptions<MailConfig> _options;
        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger<SaveFeedbackEffect> _logger;

        protected override string? ErrorNotificationMessage => "Failed to send feedback";

        public SaveFeedbackEffect(
            IEmailSender emailSender,
            IOptions<MailConfig> options,
            ITokenProvider tokenProvider,
            ILogger<SaveFeedbackEffect> logger)
            : base(logger)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override CancellationToken GetCancellationToken() => _tokenProvider.GetNewTokenAndCancelPreviousTokens();

        protected override async Task HandleAction(SaveFeedback action, IDispatcher dispatcher, CancellationToken token)
        {
            var options = _options.Value;
            try
            {
                await _emailSender.SendEmailAsync(options.FeedbackAddress, $"Feedback from Pernix from '{action.FromEmail}'", action.Feedback, action.FromEmail);
                
                dispatcher.DispatchSuccess("Thank you for your feedback.");
                
                _logger.LogInformation("Successfully sent feedback for '{User}'", action.FromEmail);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to save feedback. Feedback address: {FeedbackAddress}", options.FeedbackAddress);
                
                dispatcher.DispatchError("Failed to send feedback.");                
            }
        }
    }
}
