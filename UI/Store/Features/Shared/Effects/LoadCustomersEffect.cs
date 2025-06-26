using Blazored.LocalStorage;
using Fluxor;
using Microsoft.Extensions.Options;
using Framework.Flazor;
using Framework.Flazor.Token;
using FlazorTemplate.Configuration;
using FlazorTemplate.Models;
using FlazorTemplate.Store.Features.Shared.Actions;
using FlazorTemplate.Providers;
using Framework.Flazor.Loadable;

namespace FlazorTemplate.Store.Features.Shared.Effects
{
    /// <summary>
    /// Ensures that the first customer in the list is selected if we have loaded customers and no other customer is selected.
    /// </summary>
    public class LoadCustomersEffect : BaseEffect<LoadCustomers>
    {
        private readonly ICustomerProvider _customerProvider;
        private readonly IState<AppState> _appState;
        private readonly ILocalStorageService _localStorageService;
        private readonly IOptions<AppSettings> _options;
        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger<LoadCustomersEffect> _logger;

        protected override string? ErrorNotificationMessage => "Unable to load Customers";

        public LoadCustomersEffect(
            ICustomerProvider customerProvider,
            IState<AppState> appState,
            ILocalStorageService localStorageService,
            IOptions<AppSettings> options,
            ITokenProvider tokenProvider,
            ILogger<LoadCustomersEffect> logger)
            : base(logger)
        {
            _customerProvider = customerProvider ?? throw new ArgumentNullException(nameof(customerProvider));
            _appState = appState ?? throw new ArgumentNullException(nameof(appState));
            _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override CancellationToken GetCancellationToken() => _tokenProvider.GetNewTokenAndCancelPreviousTokens();

        protected override async Task HandleAction(LoadCustomers action, IDispatcher dispatcher, CancellationToken token)
        {
            var customers = _customerProvider.GetCustomers().ToList();

            _logger.LogInformation("Raising Action {Action} with {OrgCount} customers", nameof(LoadCustomers), customers.Count());
            dispatcher.Dispatch(new Loaded<IEnumerable<Customer>>(customers));

            var options = _options.Value;
            var state = _appState.Value;

            if (state.SelectedCustomer is null)
            {
                // If we have a customer selected in local storage, select them.
                // Otherwise, default to the first org in the list.
                if (await _localStorageService.GetItemAsStringAsync(options.SelectedCustomerIdStorageKey) is string customerId &&
                    customers.FirstOrDefault(customer => customer.Id == customerId) is Customer selectedCustomer)
                {
                    if (selectedCustomer.IsDivisionHead)
                    {
                        //select the first customer in the division
                        var selectedSubCustomer = customers.FirstOrDefault(customer => customer.ParentId == selectedCustomer.Id);
                        if (selectedSubCustomer.IsDivisionHead)
                        {
                            //select the first customer in the division
                            var selectedSubSubCustomer = customers.FirstOrDefault(customer => customer.ParentId == selectedSubCustomer.Id);
                            SetSelectedCustomer(selectedSubSubCustomer, dispatcher);
                        }
                        else
                        {
                            SetSelectedCustomer(selectedSubCustomer, dispatcher);
                        }
                    }
                    else
                    {
                        SetSelectedCustomer(selectedCustomer, dispatcher);
                    }
                }
                else if (customers?.FirstOrDefault() is Customer defaultCustomer)
                {
                    SetSelectedCustomer(defaultCustomer, dispatcher);
                }
            }
        }

        private void SetSelectedCustomer(Customer customer, IDispatcher dispatcher)
        {
            _logger.LogInformation("Selecting customer {@Customer}", customer);
            dispatcher.Dispatch(new CustomerChanged(customer));
        }
    }
}
