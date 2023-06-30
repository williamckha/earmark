using Earmark.Backend.Models;

namespace Earmark.Data.Messages
{
    public class BudgetedAmountChangedMessage
    {
        public BudgetedAmount BudgetedAmount { get; }

        public BudgetedAmountChangedMessage(BudgetedAmount budgetedAmount)
        {
            BudgetedAmount = budgetedAmount;
        }
    }
}
