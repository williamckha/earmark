using Earmark.Data.CreationSpecs;
using Earmark.Helpers.Validation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Earmark.UserControls.Dialogs
{
    public sealed partial class AddAccountDialog : ContentDialog
    {
        public static readonly DependencyProperty InputValidatorProperty =
            DependencyProperty.Register(nameof(InputValidator), typeof(IDataValidator<AccountCreationSpec>), typeof(AddAccountDialog), new PropertyMetadata(null));

        public IDataValidator<AccountCreationSpec> InputValidator
        {
            get => (IDataValidator<AccountCreationSpec>)GetValue(InputValidatorProperty);
            set => SetValue(InputValidatorProperty, value);
        }

        public static readonly DependencyProperty StartingBalanceProperty =
            DependencyProperty.Register(nameof(StartingBalance), typeof(int), typeof(AddAccountDialog), new PropertyMetadata(0));

        public int StartingBalance
        {
            get => (int)GetValue(StartingBalanceProperty);
            set => SetValue(StartingBalanceProperty, value);
        }

        public AccountCreationSpec AccountCreationSpec { get; private set; } = null;

        public AddAccountDialog()
        {
            this.InitializeComponent();
            this.XamlRoot = App.Current.Window.Content.XamlRoot;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var accountCreationSpec = new AccountCreationSpec()
            {
                AccountName = AccountNameTextBox.Text,
                IsOffBudget = (bool)IsOffBudgetCheckBox.IsChecked,
                StartingBalance = StartingBalance
            };

            if (InputValidator.Validate(accountCreationSpec, out string errorMessage))
            {
                AccountCreationSpec = accountCreationSpec;
                ErrorInfoBar.Message = string.Empty;
                ErrorInfoBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                ErrorInfoBar.Message = errorMessage;
                ErrorInfoBar.Visibility = Visibility.Visible;
                args.Cancel = true;
            }
        }
    }
}
