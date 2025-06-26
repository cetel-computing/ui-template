using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using FlazorTemplate.Authorization.Token;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

namespace FlazorTemplate.Authorization
{
    public sealed class PernixAuth
    {
        private readonly ITokenHandler _tokenHandler;
        private readonly ILogger<PernixAuth> _logger;

        public PernixAuth(
            ITokenHandler tokenHandler,
            ILogger<PernixAuth> logger)
        {
            _tokenHandler = tokenHandler ?? throw new System.ArgumentNullException(nameof(tokenHandler));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipalFromToken(string oneTimeLinkGuid)
        {
            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            var (email, _, status, error) = _tokenHandler.VerifyOneTimeToken(oneTimeLinkGuid);

            if (status)
            {
                _logger.LogInformation("One time token for {EmailAddress} is valid", email);
                claimsIdentity.AddClaims(new List<Claim>
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, "gen")
                });

                return new ClaimsPrincipal(claimsIdentity);
            }
            else
            {
                _logger.LogWarning("Login token validation for {EmailAddress} failed with {AuthenticationError}", email, error);
                throw new AuthenticationException(error);
            }

        }
    }
}
