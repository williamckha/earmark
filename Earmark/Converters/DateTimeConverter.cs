using System.Globalization;

namespace Earmark.Converters
{
    public static class DateTimeConverter
    {
        public static string MonthNumberToAbbreviation(int month, bool isUppercase)
        {
            var abbreviation = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(month).TrimEnd('.');
            return isUppercase ? abbreviation.ToUpper() : abbreviation;
        }
    }
}
