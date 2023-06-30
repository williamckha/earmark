using Earmark.Backend.Models;

namespace Earmark.Data.Messages
{
    public class BudgetedAmountResetMessage
    {
        public BudgetMonth Month { get; }

        public BudgetedAmountResetMessage(BudgetMonth month)
        {
            Month = month;
        }
    }
}
