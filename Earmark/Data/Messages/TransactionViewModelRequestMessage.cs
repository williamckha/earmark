using CommunityToolkit.Mvvm.Messaging.Messages;
using Earmark.Backend.Models;
using Earmark.ViewModels.Account;

namespace Earmark.Data.Messages
{
    public class TransactionViewModelRequestMessage : RequestMessage<TransactionViewModel>
    {
        public Transaction Transaction { get; }

        public TransactionViewModelRequestMessage(Transaction transaction)
        {
            Transaction = transaction;
        }
    }
}
