using Microsoft.Extensions.Options;
using Blazored.LocalStorage;
using Fluxor;
using FlazorTemplate.Configuration;
using FlazorTemplate.Models;
using FlazorTemplate.Store.Features.Shared.Actions;

namespace FlazorTemplate.Store.Features.Shared.Effects
{
    /// <summary>
    /// Records the new selected customer ID in local storage.
    /// </summary>
    public class CustomerChangedEffect : Effect<CustomerChanged>
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IOptions<AppSettings> _options;
        private readonly ILogger<CustomerChangedEffect> _logger;

        public CustomerChangedEffect(
            ILocalStorageService localStorageService,
            IOptions<AppSettings> options,
            ILogger<CustomerChangedEffect> logger)
        {
            _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task HandleAsync(CustomerChanged action, IDispatcher dispatcher)
        {
            if (action.Customer is Customer customer)
            {
                _logger.LogInformation("Storing selected customer {CustomerId} in local storage.", customer.Id);
                var key = _options.Value.SelectedCustomerIdStorageKey;
                await _localStorageService.SetItemAsStringAsync(key, customer.Id);
            }
        }
    }
}
