using System.Security.Claims;
using FlazorTemplate.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace FlazorTemplate.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Tests whether the <paramref name="principal"/> is in one of the adminstrative roles.
        /// </summary>
        /// <param name="principal"></param>
        /// <returns><c>true</c> if this user is in an administrative role, else <c>false</c></returns>
        public static bool IsSysAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(AuthRole.Admin);
        }

        /// <summary>
        /// Provides the value of the email claim for <paramref name="principal"/>
        /// </summary>
        /// <param name="principal"></param>
        /// <returns>string email</returns>
        public static string? UserEmail(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Provides the value of the name for <paramref name="principal"/>
        /// </summary>
        /// <param name="principal"></param>
        /// <returns>string email</returns>
        public static string? UserName(this ClaimsPrincipal principal)
        {
            return principal.Identity?.Name;
        }
    }
}
