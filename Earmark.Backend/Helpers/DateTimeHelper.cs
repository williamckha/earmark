using System;

namespace Earmark.Backend.Helpers
{
    public static class DateTimeHelper
    {
        public static (int previousMonth, int yearOfPreviousMonth) GetPreviousMonth(int month, int year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            var previousMonth = (month > 1) ? (month - 1) : 12;
            var yearOfPreviousMonth = (month > 1) ? year : (year - 1);
            return (previousMonth, yearOfPreviousMonth);
        }

        public static (int nextMonth, int yearOfNextMonth) GetNextMonth(int month, int year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            var nextMonth = (month < 12) ? (month + 1) : 1;
            var yearOfNextMonth = (month < 12) ? year : (year + 1);
            return (nextMonth, yearOfNextMonth);
        }
    }
}
