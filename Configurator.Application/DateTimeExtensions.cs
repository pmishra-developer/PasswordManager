namespace Configurator.Application
{
    public static class DateTimeExtensions
    {
        public static bool InIsRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck.Date >= startDate.Date && dateToCheck.Date < endDate.Date;
        }

        public static bool InIsRange(this DateTime dateToCheck, List<DateTimeRange> subscriptionDateRange)
        {
            foreach (var subscriptionDate in subscriptionDateRange)
            {
                if (dateToCheck.Date >= subscriptionDate.StartDate.Date && dateToCheck < subscriptionDate.EndDate.Date)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public struct DateTimeRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTimeRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
