using Fluxor;
using Blazored.LocalStorage;
using Framework.Flazor;
using Framework.Flazor.Token;
using FlazorTemplate.Store.Features.Shared.Actions;
using FlazorTemplate.Configuration;
using Microsoft.Extensions.Options;

namespace Corvid.FlazorTemplate.Shared.Effects
{
    /// <summary>
    /// Effect raised when app starts - calls initial load of orgs
    /// </summary>
    public class StoreInitializedActionEffect : BaseEffect<StoreInitializedAction>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly IOptions<AppSettings> _options;
        private readonly string? _initialCustomer;
        private readonly bool? _portal;
        private readonly ILogger<StoreInitializedActionEffect> _logger;

        public StoreInitializedActionEffect(
            ITokenProvider tokenProvider,
            ILocalStorageService localStorageService,
            IOptions<AppSettings> options,
            ILogger<StoreInitializedActionEffect> logger)
            : base(logger)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override string? ErrorNotificationMessage => "Failed to initialise customers.";

        protected override CancellationToken GetCancellationToken() => _tokenProvider.GetNewTokenAndCancelPreviousTokens();

        protected override async Task HandleAction(StoreInitializedAction action, IDispatcher dispatcher, CancellationToken token)
        {
            //if an initial customer has been set then set them in storage and set the domains for emails
            if (_initialCustomer != null)
            {
                var key = _options.Value.SelectedCustomerIdStorageKey;
                await _localStorageService.SetItemAsync(key, _initialCustomer);

            }

            _logger.LogInformation("Raising Action {Action}", nameof(LoadCustomers));
            dispatcher.Dispatch(new LoadCustomers());
        }
    }
}
