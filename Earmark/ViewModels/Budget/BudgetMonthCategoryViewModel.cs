using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Data.Messages;

namespace Earmark.ViewModels.Budget
{
    public partial class BudgetMonthCategoryViewModel : ObservableRecipient
    {
        private IAccountDetailService _accountDetailService;
        private IBudgetService _budgetService;

        private BudgetMonth _budgetMonth;
        private Category _category;

        /// <summary>
        /// The total amount of money budgeted for the category.
        /// </summary>
        public decimal TotalBudgeted
        {
            get => _budgetService.GetTotalBudgetedForMonth(_budgetMonth, _category); 
            set
            {
                if (_budgetService.GetTotalBudgetedForMonth(_budgetMonth, _category) != value)
                {
                    var budgetedAmount = _budgetService.SetBudgetedAmount(_budgetMonth, _category, value);
                    OnPropertyChanged(nameof(TotalBudgeted));
                    Messenger.Send(new BudgetedAmountChangedMessage(budgetedAmount));
                }
            }
        }

        /// <summary>
        /// The total sum of inflows and outflows for the category.
        /// </summary>
        public decimal TotalActivity => 
            _accountDetailService.GetTotalActivityForMonth(_budgetMonth.Month, _budgetMonth.Year, _category);

        /// <summary>
        /// The total amount of money available for the category.
        /// </summary>
        public decimal TotalBalance => _budgetService.GetTotalBalanceForMonth(_budgetMonth, _category);

        public BudgetMonthCategoryViewModel(
            IAccountDetailService accountDetailService, 
            IBudgetService budgetService, 
            BudgetMonth budgetMonth, 
            Category category) : base(StrongReferenceMessenger.Default)
        {
            _accountDetailService = accountDetailService;
            _budgetService = budgetService;

            _budgetMonth = budgetMonth;
            _category = category;

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<BudgetMonthCategoryViewModel, BudgetedAmountChangedMessage>(this, (r, m) =>
            {
                if (r._category == m.BudgetedAmount.Category)
                {
                    r.OnPropertyChanged(nameof(TotalBalance));
                }
            });

            Messenger.Register<BudgetMonthCategoryViewModel, BudgetedAmountResetMessage>(this, (r, m) =>
            {
                if (r._budgetMonth == m.Month)
                {
                    r.OnPropertyChanged(nameof(TotalBudgeted));
                }
                r.OnPropertyChanged(nameof(TotalBalance));
            });
        }
    }
}
