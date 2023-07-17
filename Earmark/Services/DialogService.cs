using Earmark.Backend.Services;
using Earmark.Data.CreationSpecs;
using Earmark.Helpers.Validation;
using Earmark.UserControls.Dialogs;
using System;
using System.Threading.Tasks;

namespace Earmark.Services
{
    public class DialogService : IDialogService
    {
        private IAccountService _accountService;

        public bool IsDialogOpen { get; private set; }

        public DialogService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<AccountCreationSpec> OpenAddAccountDialog()
        {
            if (!IsDialogOpen)
            {
                var dialog = new AddAccountDialog();
                dialog.InputValidator = new AccountValidator(_accountService);

                await dialog.ShowAsync();

                return dialog.AccountCreationSpec;
            }

            return null;
        }
    }
}
