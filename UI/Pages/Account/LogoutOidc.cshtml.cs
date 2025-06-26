using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FlazorTemplate.Pages.Account
{
    public class LogoutOidc : PageModel
    {
        private readonly ILogger<LogoutOidc> _logger;

        public LogoutOidc(ILogger<LogoutOidc> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            _logger.LogInformation("User signed out");
        }
    }
}
