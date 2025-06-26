using System.Security.Claims;
using FlazorTemplate.Models;

namespace FlazorTemplate.Providers
{
    public class IdentityServerCustomerProvider : ICustomerProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IList<string> _customerFilter;
        private readonly string? _initialCustomer;

        public IdentityServerCustomerProvider(IHttpContextAccessor httpContextAccessor, TokenProvider urlCustomerProvider)
        {            
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _customerFilter = urlCustomerProvider.CustomerFilter?.Split(';').ToList() ?? new List<string>();
            _initialCustomer = urlCustomerProvider.InitialCustomer;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
            {
                return Enumerable.Empty<Customer>();
            }
            else
            {
                var customers = user.Claims
                   .Where(c => c.Type == "CustomerClaimType")
                   .Select(CreateCustomer);

                return customers
                    .Where(c => !_customerFilter.Any() || _customerFilter.Contains(c.Id) || _customerFilter.Contains(c.ParentId) ||
                        (_initialCustomer != null && c.Id == _initialCustomer) ); 
            }
        }

        private Customer CreateCustomer(Claim claim)
        {
            _ = claim.Properties.TryGetValue("Name", out var name);
            _ = claim.Properties.TryGetValue("ParentGroupId", out var group);
            var isDivisionHead = claim.Properties.ContainsKey("DivisionHeadPropertyKey");
            
            var customer = new Customer
            {
                Id = claim.Value,
                Name = name,
                ParentId = group,
                IsDivisionHead = isDivisionHead
            };

            return customer;
        }
    }
}
