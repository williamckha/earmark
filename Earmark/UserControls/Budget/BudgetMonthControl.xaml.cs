using Earmark.ViewModels.Budget;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Earmark.UserControls.Budget
{
    public sealed partial class BudgetMonthControl : UserControl
    {
        public BudgetMonthViewModel ViewModel => DataContext as BudgetMonthViewModel;

        public BudgetMonthControl()
        {
            this.InitializeComponent();
        }

        public static SolidColorBrush GetCategoryActivityForegroundBrush(decimal value)
        {
            if (value == 0)
            {
                return App.Current.Resources["TextFillColorDisabledBrush"] as SolidColorBrush;
            }
            else
            {
                return App.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
            }
        }

        public static SolidColorBrush GetCategoryBalanceForegroundBrush(decimal value)
        {
            if (value < 0)
            {
                return App.Current.Resources["SystemFillColorCriticalBrush"] as SolidColorBrush;
            }
            else if (value == 0)
            {
                return App.Current.Resources["TextFillColorDisabledBrush"] as SolidColorBrush;
            }
            else
            {
                return App.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
            }
        }
    }
}
