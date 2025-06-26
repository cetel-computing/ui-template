using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using Blazored.LocalStorage;
using Blazored.Modal;
using BlazorTable;
using Flazor.Extensions;
using Fluxor;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Polly;
using Polly.Extensions.Http;
using FlazorTemplate.Authorization;
using FlazorTemplate.Configuration;
using FlazorTemplate.Providers;
using FlazorTemplate.Services.Data;
using FlazorTemplate.Services.Email;
using FlazorTemplate.Services.Modals;
using FlazorTemplate.Services.Navigation;

namespace FlazorTemplate
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Jitterer used to add random offset for exponential retry strategy.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly#add-a-jitter-strategy-to-the-retry-policy"/>
        private static readonly Random Jitterer = new Random();

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services.AddRazorPages(o =>
            {
                //Set the whole to site to require authentication.
                o.Conventions.AuthorizeFolder("/");
                //Allow anonymous to 'SignIn'.
                o.Conventions.AllowAnonymousToPage("/SignIn");
                o.Conventions.AllowAnonymousToPage("/Error");
            });

            if (_environment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            services.AddFluxor(options =>
            {
                var fluxorConfig = options.ScanAssemblies(Assembly.GetExecutingAssembly());

                options
                    .AutoregisterFeatures()
                    .AddErrorHandling()
                    .AddInMemoryTokenProvider();

                if (_environment.IsDevelopment())
                {
                    fluxorConfig.UseReduxDevTools();
                }
            });

            services.AddServerSideBlazor()
                .AddHubOptions(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
                });

            var oidcSection = _configuration.GetSection("OIDC");
            var authConfig = oidcSection.Get<AuthConfig>();
            var mailSection = _configuration.GetSection("Mail");
            services
                .Configure<AppSettings>(_configuration.GetSection("AppSettings"))
                .Configure<MailConfig>(mailSection)
                .Configure<AuthConfig>(oidcSection);

            services
                .AddBlazoredLocalStorage()
                .AddBlazoredModal()
                .AddBlazorTable();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add("name", ClaimTypes.Name);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[ClaimTypes.Role] = "role";

            services
                .AddAuthorizationPolicies()
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                })
                .AddCookie(o =>
                {
                    o.AccessDeniedPath = "/SignIn";

                    //Will automatically refresh the reference token if required.
                    //o.AutoRefreshAccessTokenFromCookie();

                    var cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
                    o.SessionStore = new MemoryCacheTicketStore(cache);
                })
                .AddOpenIdConnect(IdentityConstants.ApplicationScheme, o =>
                {
                    //Open Id Connect settings.
                    o.Authority = authConfig.Authority;
                    o.ClientId = authConfig.ClientId;
                    o.ClientSecret = authConfig.ClientSecret;
                    o.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.GetClaimsFromUserInfoEndpoint = true;
                    o.SaveTokens = true;
                    o.SignedOutCallbackPath = "/signout-callback-oidc";
                    o.SignedOutRedirectUri = "/account/logoutoidc";

                    //Required scopes.
                    o.Scope.Add(OidcConstants.StandardScopes.OpenId);
                    o.Scope.Add(OidcConstants.StandardScopes.Profile);
                    o.Scope.Add(OidcConstants.StandardScopes.Email);
                    o.Scope.Add("roles"); // Permits access to "role" claims.
                    //Required to get a refresh token for renewing the access token.
                    o.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);

                    //User cancel or remote failure event. Redirect to the 'sign in' page.
                    o.Events.OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/Error");
                        context.HandleResponse();

                        return Task.CompletedTask;
                    };

                    var httpClient = new HttpClient();
                    //o.AddCustomerGroupDataToPrincipal(new GroupProvider(authConfig, httpClient), IrAdmin);
                });

            services
                .AddTransient<IEmailSender, EmailSender>()
                .AddTransient<INavigator, Navigator>()
                .AddTransient<IModalActions, ModalActions>()
                .AddScoped<ICustomerProvider, IdentityServerCustomerProvider>()
                .AddScoped<TokenProvider>();

            // Register FluentEmail
            // See (https://github.com/lukencode/FluentEmail)
            var mailConfig = mailSection.Get<MailConfig>() ?? throw new Exception("Could not read Mail config");
            services
                .AddFluentEmail(mailConfig.Sender, mailConfig.SenderName)
                .AddRazorRenderer()
                .AddSmtpSender(() =>
                {
                    var client = new SmtpClient(mailConfig.MailServer, mailConfig.MailPort);
                    client.EnableSsl = mailConfig.EnableSsl;
                    return client;
                });

            //Required for reverse proxy support.
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedHost |
                    ForwardedHeaders.XForwardedProto;
            });

            services.AddHttpClient();

            //add api service
            //services
            //    .AddHttpContextAccessor()
            //    .AddHttpClient<IPernixApi, PernixApi>()
            //    .AddPolicyHandler(BackoffRetryPolicy);

            //services
            //   .AddTransient<IPernixDataAccessService, PernixDataAccessService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //add forward headers to allow uris to be correctly generated behind proxies
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub(opts => opts.WebSockets.CloseTimeout = new TimeSpan(1, 1, 1));
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private static IAsyncPolicy<HttpResponseMessage> BackoffRetryPolicy(IServiceProvider serviceProvider, HttpRequestMessage requestMessage)
        {
            // Adds an exponential backoff, with jitter
            // See https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly#add-a-jitter-strategy-to-the-retry-policy
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(5, retryAttempt => System.TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + System.TimeSpan.FromMilliseconds(Jitterer.Next(0, 100)));
        }
    }
}
