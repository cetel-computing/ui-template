using Fluxor;
using Framework.Flazor;
using Framework.Flazor.Token;
using FlazorTemplate.Providers;
using FlazorTemplate.Store.Features.Shared.Actions;

namespace FlazorTemplate.Shared.Effects
{
    public class CustomerIdChangedEffect : BaseEffect<CustomerIdChanged>
    {
        private readonly ICustomerProvider _customerProvider;
        private readonly ITokenProvider _tokenProvider;
        private readonly ILogger<CustomerIdChangedEffect> _logger;

        protected override string? ErrorNotificationMessage => "Unable to change customer id";

        public CustomerIdChangedEffect(
            ICustomerProvider customerProvider,
            ITokenProvider tokenProvider,
            ILogger<CustomerIdChangedEffect> logger)
            : base(logger)
        {
            _customerProvider = customerProvider ?? throw new ArgumentNullException(nameof(customerProvider));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override CancellationToken GetCancellationToken() => _tokenProvider.GetNewTokenAndCancelPreviousTokens();

        protected override async Task HandleAction(CustomerIdChanged action, IDispatcher dispatcher, CancellationToken token)
        {
            var customer = _customerProvider.GetCustomers().FirstOrDefault(c => c.Id == action.CustomerId);

            if (customer == null)
            {
                dispatcher.DispatchError($"Could not find customer {action.CustomerId}.");
            }
            else
            {
                dispatcher.Dispatch(new CustomerChanged(customer));
            }
        }
    }
}
