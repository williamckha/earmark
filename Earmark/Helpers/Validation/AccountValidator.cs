using Earmark.Backend.Services;
using Earmark.Data.CreationSpecs;
using System.Linq;

namespace Earmark.Helpers.Validation
{
    public class AccountValidator : IDataValidator<AccountCreationSpec>
    {
        private IAccountService _accountService;

        public AccountValidator(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public bool Validate(AccountCreationSpec accountCreationSpec, out string errorMessage)
        {
            if (string.IsNullOrEmpty(accountCreationSpec.AccountName))
            {
                errorMessage = "AccountNameIsEmptyErrorMessage".GetLocalizedResource();
                return false;
            }

            if (_accountService.GetAccounts().Any(x => x.Name == accountCreationSpec.AccountName))
            {
                errorMessage = "AccountAlreadyExistsErrorMessage".GetLocalizedResource();
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
