using Earmark.Backend.Models;
using Earmark.ViewModels.Account;

namespace Earmark.Data.Messages
{
    public class TransferTransactionChangedMessage
    {
        public TransactionViewModel OldTransferTransaction { get; }

        public Transaction NewTransferTransaction { get; }

        public TransferTransactionChangedMessage(
            TransactionViewModel oldTransferTransaction, Transaction newTransferTransaction)
        {
            OldTransferTransaction = oldTransferTransaction;
            NewTransferTransaction = newTransferTransaction;
        }
    }
}
