using Earmark.Backend.Database;
using Earmark.Backend.Services;
using Earmark.Services;
using Earmark.ViewModels;
using Earmark.ViewModels.Account;
using Earmark.ViewModels.Budget;
using Earmark.Views;
using EntityFramework.DbContextScope;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace Earmark
{
    public partial class App : Application
    {
        private MainWindow _window;

        /// <summary>
        /// Gets the current application instance.
        /// </summary>
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Gets the window for the application instance.
        /// </summary>
        public MainWindow Window => _window;

        /// <summary>
        /// Gets the IServiceProvider instance for the application instance.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            Services = ConfigureServices();

            var dbContextScopeFactory = Services.GetService<IDbContextScopeFactory>();
            using (var dbContextScope = dbContextScopeFactory.Create())
            {
                dbContextScope.DbContexts.Get<AppDbContext>().Database.Migrate();
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var navigationService = Services.GetService<INavigationService>();
            navigationService.RegisterViewModelForView<BudgetViewModel, BudgetView>();
            navigationService.RegisterViewModelForView<ReportsViewModel, ReportsView>();
            navigationService.RegisterViewModelForView<AccountViewModel, AccountView>();

            _window = new MainWindow();
            _window.ViewModel = Services.GetService<MainViewModel>();
            _window.Activate();
        }

        /// <summary>
        /// Configures a new IServiceProvider instance with the required services.
        /// </summary>
        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                
                .AddTransient<MainViewModel>()
                .AddTransient<BudgetViewModel>()
                .AddTransient<ReportsViewModel>()
                .AddTransient<AccountViewModel>()

                .AddSingleton<IDialogService, DialogService>()
                .AddSingleton<INavigationService, NavigationService>()

                .AddSingleton<IDbContextScopeFactory, DbContextScopeFactory>()
                .AddSingleton<IAccountService, AccountService>()
                .AddSingleton<IBudgetService, BudgetService>()
                .AddSingleton<ICategoriesService, CategoriesService>()
                .AddSingleton<IPayeeService, PayeeService>()

                .BuildServiceProvider();
        }
    }
}
