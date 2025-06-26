namespace FlazorTemplate.Configuration
{
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the key used to store the user's currently selected customer.
        /// </summary>
        public string SelectedCustomerIdStorageKey { get; set; } = "customer.id";

        /// <summary>
        /// Gets or sets the key used to store the user's currently selected customer.
        /// </summary>
        public string SelectedOrgIdStorageKey { get; set; } = "customer.org.id";

        /// <summary>
        /// Gets or sets the key used to store the user's currently selected domain.
        /// </summary>
        public string SelectedDomainIdStorageKey { get; set; } = "customer.domain.id";

        public string ApiUrl { get; set; }
    }
}
