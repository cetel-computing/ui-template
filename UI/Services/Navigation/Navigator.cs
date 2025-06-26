using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FlazorTemplate.Services.Navigation
{
    public class Navigator : INavigator
    {
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _runtime;
        private readonly ILogger<Navigator> _logger;
        private readonly IDispatcher _dispatcher;

        public Navigator(NavigationManager navigationManager, IJSRuntime runtime, IDispatcher dispatcher, ILogger<Navigator> logger)
        {
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task NavigateNewPage(string url, string uuId)
        {
            // There may be tooltips still visible on the page. Popper.js will not clear them automatically, so
            // ensure they are cleared before we navigate.
            await _runtime.InvokeVoidAsync("ClearTooltips");
            //open in a new page
            await _runtime.InvokeAsync<object>("open", url, uuId);
        }

        private async Task Navigate(string url)
        {
            // There may be tooltips still visible on the page. Popper.js will not clear them automatically, so
            // ensure they are cleared before we navigate.
            await _runtime.InvokeVoidAsync("ClearTooltips");
            _navigationManager.NavigateTo(url);
        }
    }
}
