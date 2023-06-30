using Earmark.ViewModels.Account;
using Microsoft.UI.Xaml.Controls;

namespace Earmark.UserControls.Account
{
    public sealed partial class TransactionTable : UserControl
    {
        public AccountViewModel ViewModel => DataContext as AccountViewModel;

        public TransactionTable()
        {
            this.InitializeComponent();
        }
    }
}
