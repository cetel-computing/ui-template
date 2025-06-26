using FlazorTemplate.Models;

namespace FlazorTemplate.Store.Features.Shared.Actions
{
    /// <summary>
    /// Action fired when a customer is selected.
    /// </summary>
    public class CustomerChanged
    {
        public CustomerChanged(Customer customer)
        {
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        }

        public Customer Customer { get; }

        public override string ToString()
        {
            return $"{nameof(CustomerChanged)}: {Customer?.Id}";
        }
    }
}
