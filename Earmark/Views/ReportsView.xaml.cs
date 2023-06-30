using Earmark.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Earmark.Views
{
    public sealed partial class ReportsView : Page
    {
        public ReportsViewModel ViewModel => DataContext as ReportsViewModel;

        public ReportsView()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetService<ReportsViewModel>();
        }
    }
}
