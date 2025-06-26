namespace FlazorTemplate.Store.Features.Shared.Actions
{
    /// <summary>
    /// Raised when the date range is changed.
    /// </summary>
    public class DateRangeChanged
    {
        public DateRangeChanged(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTimeOffset StartDate { get; }

        public DateTimeOffset EndDate { get; }
    }
}
