using Earmark.Data.CreationSpecs;
using System.Threading.Tasks;

namespace Earmark.Services
{
    public interface IDialogService
    {
        bool IsDialogOpen { get; }

        Task<AccountCreationSpec> OpenAddAccountDialog();
    }
}
