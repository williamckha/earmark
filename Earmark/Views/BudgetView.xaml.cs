using Earmark.ViewModels.Budget;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Earmark.Views
{
    public sealed partial class BudgetView : Page
    {
        public BudgetViewModel ViewModel => DataContext as BudgetViewModel;

        public static readonly DependencyProperty BudgetMonthWidthProperty =
            DependencyProperty.Register(nameof(BudgetMonthWidth), typeof(double), typeof(BudgetView), new PropertyMetadata(null));

        public double BudgetMonthWidth
        {
            get => (double)GetValue(BudgetMonthWidthProperty);
            set => SetValue(BudgetMonthWidthProperty, value);
        }

        public BudgetView()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetService<BudgetViewModel>();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.IsActive = false;
        }
    }
}
