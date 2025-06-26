using FlazorTemplate.Models;

namespace FlazorTemplate.Providers
{
    /// <summary>
    /// Provides access to the customers that the user has permissions to administer.
    /// </summary>
    public interface ICustomerProvider
    {
        /// <summary>
        /// Gets the customers the user has permission to administer.
        /// </summary>
        IEnumerable<Customer> GetCustomers();
    }
}
