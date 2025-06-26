using Microsoft.AspNetCore.Authorization;
using static FlazorTemplate.Authorization.AuthRole;

namespace FlazorTemplate.Authorization
{
    /// <summary>
    /// Defines the supported security policies within the Pernix API.
    /// </summary>
    public static class AuthorizationPolicies
    {
        /// <summary>
        /// Authorises any admin or higher
        /// </summary>
        public const string AdminPolicy = "admin-policy";

        /// <summary>
        /// Adds the authorization policies to the service.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns><c>services</c>, for chaining</returns>
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            return services
                .AddAuthorization(options =>
                {
                    options.AddPolicy(AdminPolicy, RequireRole(Admin));
                });
        }

        private static Action<AuthorizationPolicyBuilder> RequireRole(params string[] roles)
        {
            return builder => builder.RequireRole(roles);
        }
    }

    public class AuthRole
    {
        public const string Admin = "admin.system";

        public static readonly AuthRole PernixAdminRole = new AuthRole
        {
            Val = "admin.system"
        };

        public string Val { get; set; }

        public override string ToString()
        {
            return Val;
        }   
    }
}
