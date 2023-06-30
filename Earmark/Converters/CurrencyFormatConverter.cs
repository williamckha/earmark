using Windows.Globalization.NumberFormatting;

namespace Earmark.Converters
{
    public static class CurrencyFormatConverter
    {
        public static DecimalFormatter Formatter { get; } = new()
        {
            IntegerDigits = 1,
            FractionDigits = 2,
            IsDecimalPointAlwaysDisplayed = true,
            IsGrouped = true,
            NumberRounder = new IncrementNumberRounder()
            {
                Increment = 0.01,
                RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
            }
        };

        public static string DecimalToString(decimal amount)
        {
            return Formatter.Format(decimal.ToDouble(amount));
        }

        public static string NegatedDecimalToString(decimal amount)
        {
            return Formatter.Format(decimal.ToDouble(-amount));
        }
    }
}
