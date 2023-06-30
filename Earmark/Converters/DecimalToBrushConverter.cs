using Microsoft.UI.Xaml.Media;

namespace Earmark.Converters
{
    public static class DecimalToBrushConverter
    {
        public static SolidColorBrush GetConditionalForegroundBrush(decimal value)
        {
            if (value > 0)
            {
                return App.Current.Resources["SystemFillColorSuccessBrush"] as SolidColorBrush;
            }
            else if (value < 0)
            {
                return App.Current.Resources["SystemFillColorCriticalBrush"] as SolidColorBrush;
            }
            else
            {
                return App.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
            }
        }

        public static SolidColorBrush GetConditionalWithFadedZeroForegroundBrush(decimal value)
        {
            if (value > 0)
            {
                return App.Current.Resources["SystemFillColorSuccessBrush"] as SolidColorBrush;
            }
            else if (value < 0)
            {
                return App.Current.Resources["SystemFillColorCriticalBrush"] as SolidColorBrush;
            }
            else
            {
                return App.Current.Resources["TextFillColorDisabledBrush"] as SolidColorBrush;
            }
        }

        public static SolidColorBrush GetRedWhenNegativeForegroundBrush(decimal value)
        {
            if (value < 0)
            {
                return App.Current.Resources["SystemFillColorCriticalBrush"] as SolidColorBrush;
            }
            else
            {
                return App.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
            }
        }
    }
}
