using CommunityToolkit.WinUI.UI;
using Earmark.ViewModels.Budget;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Earmark.UserControls.Budget
{
    public sealed partial class BudgetCategoriesControl : UserControl
    {
        public BudgetViewModel ViewModel => DataContext as BudgetViewModel;

        public BudgetCategoriesControl()
        {
            this.InitializeComponent();
        }

        private void CategoryGroupRow_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var categoryGroupRow = sender as FrameworkElement;
            var addCategoryButton = categoryGroupRow.FindChild("AddCategoryButton");
            addCategoryButton.Visibility = Visibility.Visible;
        }

        private void CategoryGroupRow_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var categoryGroupRow = sender as FrameworkElement;
            var addCategoryButton = categoryGroupRow.FindChild("AddCategoryButton");
            addCategoryButton.Visibility = Visibility.Collapsed;
        }
    }
}
