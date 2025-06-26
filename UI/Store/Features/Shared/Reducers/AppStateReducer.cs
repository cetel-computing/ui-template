using Fluxor;
using Framework.Flazor.Loadable;
using FlazorTemplate.Models;
using FlazorTemplate.Store.Features.Shared.Actions;

namespace FlazorTemplate.Store.Features.Shared.Reducers
{
    public class AppStateReducer
    {
        [ReducerMethod]
        public static AppState Reduce(AppState state, ILoadStatus<IEnumerable<Customer>> action) =>
            state with
            {
                Customers = action.ToLoadable()
            };

        [ReducerMethod]
        public static AppState Reduce(AppState state, CustomerChanged action) =>
            state with
            {
                SelectedCustomer = action.Customer,
                SelectedCustomerId = action.Customer?.Id
            };

        [ReducerMethod]
        public static AppState Reduce(AppState state, DateRangeChanged action) =>
            state with
            {
                SelectedStartDate = action.StartDate,
                SelectedEndDate = action.EndDate
            };
    }
}
