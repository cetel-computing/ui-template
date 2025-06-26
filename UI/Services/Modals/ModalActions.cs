using Blazored.Modal;
using Blazored.Modal.Services;
using Fluxor;
using FlazorTemplate.Shared;
using FlazorTemplate.Store.Features.Modals.Actions;

namespace FlazorTemplate.Services.Modals
{
    public class ModalActions : IModalActions
    {
        private readonly IDispatcher _dispatcher;
        private IModalService _modal;

        public ModalActions(IDispatcher dispatcher, ILogger<ModalActions> logger, IModalService modal)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _modal = modal ?? throw new ArgumentNullException(nameof(modal));
        }

        public async Task ShowFeedbackDialog(string email)
        {
            var modal = _modal.Show<FeedbackDialog>("Contact Us");

            var result = await modal.Result;

            if (!result.Cancelled)
            {
                var feedback = (string)result.Data;

                _dispatcher.Dispatch(new SaveFeedback(email, feedback));
            }
        }

    }
}
