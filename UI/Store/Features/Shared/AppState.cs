using Framework.Flazor;
using Framework.Flazor.Autoregister;
using FlazorTemplate.Models;

namespace FlazorTemplate.Store.Features.Shared
{
    [Feature("AppState")]
    public record AppState
    {
        public Loadable<IEnumerable<Customer>> Customers { get; init; } = new ();

        public string? SelectedOrgId { get; init; }

        public string? SelectedDomainId { get; init; }

        public string? SelectedCustomerId { get; init; }

        public Customer? SelectedCustomer { get; init; }

        public DateTimeOffset SelectedStartDate { get; set; } = DateTime.Today.AddDays(-6);

        public DateTimeOffset SelectedEndDate { get; set; } = DateTime.Today.AddDays(1).AddTicks(-1);
    }
}
