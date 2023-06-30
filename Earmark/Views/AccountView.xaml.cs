using Earmark.Data.Navigation;
using Earmark.ViewModels.Account;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Earmark.Views
{
    public sealed partial class AccountView : Page
    {
        public AccountViewModel ViewModel => DataContext as AccountViewModel;

        public AccountView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = App.Current.Services.GetService<AccountViewModel>();
            ViewModel.LoadAccounts(e.Parameter as AccountGroup);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.IsActive = false;
        }
    }
}
