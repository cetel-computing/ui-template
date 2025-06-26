using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTable;
using BlazorTable.Components.ServerSide;
using BlazorTable.Interfaces;
using Fluxor;

namespace Corvid.Pernix.UserUI.Providers
{
    public class ServerSideFilteredDataLoader : IDataLoader<Email>
    {
        private readonly IState<EmailState> _state;
        private readonly IState<EmailQueryState> _queryState;
        private readonly ITable<Email> _table;
        private readonly IApi _apiService;
        private readonly IDispatcher _dispatcher;

        public ServerSideFilteredDataLoader(
            IState<EmailState> state,
            IState<EmailQueryState> queryState,
            ITable<Email> table,
            IApiDataAccessService apiService,
            IDispatcher dispatcher)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _queryState = queryState ?? throw new ArgumentNullException(nameof(queryState));
            _table = table ?? throw new ArgumentNullException(nameof(table));
            _apiService = apiService.Client;
            _dispatcher = dispatcher;
        }

        public async Task<PaginationResult<Email>> LoadDataAsync(FilterData parameters)
        {
            var state = _state.Value;

            var pageSize = parameters?.Top ?? state.EmailPageSize;
            var skip = parameters?.Skip ?? 0;

            if (pageSize != state.EmailPageSize)
            {
                skip = 0;
                _dispatcher.Dispatch(new EmailPageSizeChanged(pageSize));
                await _table.FirstPageAsync();
            }

            var emptyResult = new PaginationResult<Email>
            {
                Records = new List<Email>(),
                Skip = 0,
                Total = 0,
                Top = 0
            };

            var tab = state.SelectedTab;
            var filters = _table?.Columns.Where(x => x.Filter != null).ToList();
            var queryFilters = _queryState.Value.Filters;

            var previous = _queryState.Value.EmailQuery;

            var query = new EmailQuery
            {
                Limit = pageSize,
                Skip = skip,
                Filters = new List<QueryFilter>(),
                AndFilters = new List<QueryFilter>(),
                NotFilters = new List<QueryFilter>(),
                EmailTypes = new List<EmailType>()
            };

            //sorting applied
            if (!string.IsNullOrEmpty(parameters?.OrderBy))
            {
                var orderBy = parameters.OrderBy.Split(" ");
                if (orderBy.Length == 2)
                {
                    if (orderBy[0] == "Classification")
                    {
                        query.SortBy = "Quarantined";
                    }
                    else if (orderBy[0] == "LocalTimeDate")
                    {
                        query.SortBy = "LocalTime";
                    }
                    else
                    {
                        query.SortBy = orderBy[0] + ".keyword";
                    }

                    query.SortDirection = orderBy[1] == "desc" ? SortOrder.Desc : SortOrder.Asc;
                }
            }

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    var filterTitle = filter.Title;
                    var lambdaExpressionString = filter.Filter.Body.ToString();

                    if (filterTitle == "Date")
                    {
                        //date filetrs contain 2, 3 or 4 queries
                        var queries = lambdaExpressionString.Split(" AndAlso ");

                        if (queries.Length == 2)
                        {
                            //single date condition
                            query.AddDateFilters(queries[1], "LocalTime");
                        }
                        if (queries.Length == 3)
                        {
                            //double date query with or
                            query.AddDateFilters(queries[1], "LocalTime", false);
                            query.AddDateFilters(queries[2], "LocalTime", false);
                        }
                        if (queries.Length == 4)
                        {
                            //double date query with and
                            query.AddDateFilters(queries[1], "LocalTime");
                            query.AddDateFilters(queries[3], "LocalTime");
                        }
                    }
                    else if (filterTitle == "From")
                    {
                        query.AddFilters(lambdaExpressionString, "EnvelopeFrom.keyword");
                    }
                    else if (filterTitle == "To")
                    {
                        query.AddFilters(lambdaExpressionString, "Recipients.keyword");
                    }
                    else if (filterTitle == "Subject")
                    {
                        query.AddFilters(lambdaExpressionString, "Subject");
                    }
                    
                }
            }

            //global search query
            if (parameters?.Query != null && parameters?.Query != "")
            {
                var value = parameters.Query.ToLower();

                query.Filters.Add(new QueryFilter() { Property = "LocalTime", Value = value });
                query.Filters.Add(new QueryFilter() { Property = "EnvelopeFrom.keyword", Value = "*" + value + "*" });
                query.Filters.Add(new QueryFilter() { Property = "Recipients.keyword", Value = "*" + value + "*" });
                query.Filters.Add(new QueryFilter() { Property = "Subject", Value = "*" + value + "*" });

            }

            query.FromDate ??= state.SelectedStartDate.DateTime;
            query.ToDate ??= state.SelectedEndDate.DateTime;

            var queryRunTime = DateTime.UtcNow;
            var runNewQury = false;

            if (previous == null || queryRunTime > previous.QueryRunDateTime.AddMinutes(2))
            {
                runNewQury = true;
            }
            else
            {
                var previousQuery = previous.Query;

                //check the query against the previous one, only run the query if different.
                if (query.DomainId != previousQuery.DomainId ||
                    query.FromDate != previousQuery.FromDate ||
                    query.ToDate != previousQuery.ToDate ||
                    query.EmailType != previousQuery.EmailType ||
                    query.Limit != previousQuery.Limit ||
                    query.Skip != previousQuery.Skip ||
                    query.SortBy != previousQuery.SortBy ||
                    query.SortDirection != previousQuery.SortDirection ||
                    query.Filters.Count != previousQuery.Filters.Count ||
                    (query.Filters.Count == previousQuery.Filters.Count &&
                        query.Filters.Select(f => f.Property + f.Value).FirstOrDefault() != previousQuery.Filters.Select(f => f.Property + f.Value).FirstOrDefault()) ||
                    query.AndFilters.Count != previousQuery.AndFilters.Count ||
                    (query.AndFilters.Count == previousQuery.AndFilters.Count &&
                        query.AndFilters.Select(f => f.Property + f.Value).FirstOrDefault() != previousQuery.AndFilters.Select(f => f.Property + f.Value).FirstOrDefault()) ||
                    query.NotFilters.Count != previousQuery.NotFilters.Count ||
                    (query.NotFilters.Count == previousQuery.NotFilters.Count &&
                        query.NotFilters.Select(f => f.Property + f.Value).FirstOrDefault() != previousQuery.NotFilters.Select(f => f.Property + f.Value).FirstOrDefault()) ||
                    query.EmailTypes.Count != previousQuery.EmailTypes.Count)
                {
                    runNewQury = true;
                }
            }

            if (runNewQury)
            {
                var emails = await _apiService.SearchUserArchivesAsync(query);

                if (emails != null)
                {
                    var emailQuery = new QueryEmails { Query = query, QueryRunDateTime = queryRunTime, Emails = emails };

                    _dispatcher.Dispatch(new QueryChanged(emailQuery));

                    return new PaginationResult<Email>
                    {
                        Records = emails.Data,
                        Skip = skip,
                        Total = Convert.ToInt32(emails.TotalCount),
                        Top = pageSize
                    };
                }
                else
                {
                    return emptyResult;
                }
            }
            else
            {
                return new PaginationResult<Email>
                {
                    Records = previous.Emails.Data,
                    Skip = skip,
                    Total = Convert.ToInt32(previous.Emails.TotalCount),
                    Top = pageSize
                };
            }
        }
    }
}
