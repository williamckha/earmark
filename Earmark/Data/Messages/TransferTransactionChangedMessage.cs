using Earmark.Backend.Models;

namespace Earmark.Data.Messages
{
    public class TransferTransactionChangedMessage 
    {
        public Transaction OldTransferTransaction { get; }

        public Transaction NewTransferTransaction { get; }

        public TransferTransactionChangedMessage(
            Transaction oldTransferTransaction, Transaction newTransferTransaction)
        {
            OldTransferTransaction = oldTransferTransaction;
            NewTransferTransaction = newTransferTransaction;
        }
    }
}
