using Microsoft.UI.Xaml.Data;
using System;

namespace Earmark.Converters
{
    public class CurrencyIntegerToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int currencyInteger)
            {
                return CurrencyIntegerToDoubleConverterHelper.Convert(currencyInteger);
            }
            return 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is double currencyDouble && double.IsNormal(currencyDouble))
            {
                return CurrencyIntegerToDoubleConverterHelper.ConvertBack(currencyDouble);
            }
            return 0;
        }
    }

    public static class CurrencyIntegerToDoubleConverterHelper
    {
        public static double Convert(int currencyInteger) => System.Convert.ToDouble(currencyInteger) / 100d;
        
        public static int ConvertBack(double currencyDouble) => System.Convert.ToInt32(currencyDouble * 100d);
    }
}
