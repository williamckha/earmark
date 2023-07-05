using Microsoft.UI.Xaml.Media;

namespace Earmark.Converters
{
    public static class IntegerToBrushConverter
    {
        public static SolidColorBrush GetConditionalForegroundBrush(int value)
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

        public static SolidColorBrush GetConditionalWithFadedZeroForegroundBrush(int value)
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

        public static SolidColorBrush GetRedWhenNegativeForegroundBrush(int value)
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
