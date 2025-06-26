using Microsoft.Rest;
using Fluxor;
using Framework.Flazor;

namespace FlazorTemplate.Extensions
{
    public static class RestApiResponseExtensions
    {
        /// <summary>
        /// Tests whether the <paramref name="apiResult"/> is successful and dispatches appropriate actions.
        /// </summary>
        /// <param name="apiResult"></param>
        /// <param name="dispatcher"></param>
        public static void RaiseSuccessOrError(this HttpOperationResponse? apiResult, IDispatcher dispatcher, string successMessage)
        {
            if (apiResult == null)
            {
                dispatcher.DispatchError("Api error");
            }
            else if (apiResult.Response.StatusCode == System.Net.HttpStatusCode.OK
                || apiResult.Response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                dispatcher.DispatchSuccess(successMessage);
            }
            else
            {
                dispatcher.DispatchError(apiResult.Response.ReasonPhrase);
            }
        }
    }
}
