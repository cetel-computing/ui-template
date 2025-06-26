using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FlazorTemplate.Pages.Account
{
    /// <summary>
    /// The login page causes user to perform a login to Identity Server.
    /// </summary>
    public class Login : PageModel
    {
        private readonly ILogger<Login> _logger;

        public Login(ILogger<Login> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnGet(string redirectUri)
        {
            await HttpContext.ChallengeAsync(IdentityConstants.ApplicationScheme, new AuthenticationProperties { RedirectUri = redirectUri });
            _logger.LogInformation("User signed in");
        }
    }
}
