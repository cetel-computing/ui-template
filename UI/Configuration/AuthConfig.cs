namespace FlazorTemplate.Configuration
{
    public class AuthConfig
    {
        //
        // Summary:
        //     The identity server - e.g. https://auth.xxxx.co.uk.
        public string Authority { get; set; }

        //
        // Summary:
        //     The client configured in the identity server.
        public string ClientId { get; set; }

        //
        // Summary:
        //     The shared-secret the client uses to authenticate themselves with the authority.
        public string ClientSecret { get; set; }

        //
        // Summary:
        //     Provides an offset to the access-token expiry time.
        public TimeSpan AccessTokenTimeout { get; set; } = TimeSpan.FromMinutes(5.0);

        /// <summary>
        /// Gets or sets the claim that marks a company as a division head.
        /// </summary>
        public string DivisionHeadPropertyKey { get; set; } = "group.division";

        /// <summary>
        /// Gets or sets the claim that marks node as hidden in the portal.
        /// </summary>
        public string HiddenNodePropertyKey { get; set; } = "group.root";
    }
}