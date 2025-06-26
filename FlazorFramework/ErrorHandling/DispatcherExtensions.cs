using Fluxor;

namespace Framework.Flazor
{
    public static class DispatcherExtensions
    {
        public static void DispatchError(this IDispatcher dispatcher, string errorMessage)
        {
            dispatcher.Dispatch(new Error { Message = errorMessage });
        }

        public static void DispatchSuccess(this IDispatcher dispatcher, string successMessage)
        {
            dispatcher.Dispatch(new Success { Message = successMessage });
        }
    }
}
