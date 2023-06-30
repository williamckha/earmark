using Microsoft.UI.Xaml.Data;
using System;

namespace Earmark.Converters
{
    public class DecimalToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal decimalValue)
            {
                return decimal.ToDouble(decimalValue);
            }
            return double.NaN;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is double doubleValue && double.IsNormal(doubleValue))
            {
                return System.Convert.ToDecimal(doubleValue);
            }
            return decimal.Zero;
        }
    }
}
