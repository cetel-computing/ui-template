namespace FlazorTemplate.Store.Features.Shared.Actions
{
    /// <summary>
    /// Raised when the customer ID is changed.
    /// </summary>
    public class CustomerIdChanged
    {
        public CustomerIdChanged(string customerId)
        {
            CustomerId = customerId;
        }

        public string CustomerId { get; }
    }
}
