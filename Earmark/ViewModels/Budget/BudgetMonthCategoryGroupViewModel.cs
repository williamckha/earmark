using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Data.Messages;
using System.Collections.ObjectModel;

namespace Earmark.ViewModels.Budget
{
    public partial class BudgetMonthCategoryGroupViewModel : ObservableRecipient
    {
        private IAccountDetailService _accountDetailService;
        private IBudgetService _budgetService;

        private BudgetMonth _budgetMonth;
        private CategoryGroup _categoryGroup;

        /// <summary>
        /// The categories contained in the category group.
        /// </summary>
        public ObservableCollection<BudgetMonthCategoryViewModel> Categories { get; }

        /// <summary>
        /// The total amount of money budgeted for the category group.
        /// </summary>
        public decimal TotalBudgeted => _budgetService.GetTotalBudgetedForMonth(_budgetMonth, _categoryGroup);

        /// <summary>
        /// The total sum of inflows and outflows for the category group.
        /// </summary>
        public decimal TotalActivity => 
            _accountDetailService.GetTotalActivityForMonth(_budgetMonth.Month, _budgetMonth.Year, _categoryGroup);

        /// <summary>
        /// The total amount of money available for the category group.
        /// </summary>
        public decimal TotalBalance => _budgetService.GetTotalBalanceForMonth(_budgetMonth, _categoryGroup);

        public BudgetMonthCategoryGroupViewModel(
            IAccountDetailService accountDetailService,
            IBudgetService budgetService, 
            BudgetMonth budgetMonth, 
            CategoryGroup categoryGroup) : base(StrongReferenceMessenger.Default)
        {
            _accountDetailService = accountDetailService;
            _budgetService = budgetService;

            _budgetMonth = budgetMonth;
            _categoryGroup = categoryGroup;

            Categories = new ObservableCollection<BudgetMonthCategoryViewModel>();

            foreach (var category in _categoryGroup.Categories)
            {
                Categories.Add(new BudgetMonthCategoryViewModel(
                    _accountDetailService, _budgetService, _budgetMonth, category));
            }

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<BudgetMonthCategoryGroupViewModel, BudgetedAmountChangedMessage>(this, (r, m) =>
            {
                if (r._categoryGroup == m.BudgetedAmount.Category.Group)
                {
                    r.OnPropertyChanged(nameof(TotalBudgeted));
                    r.OnPropertyChanged(nameof(TotalBalance));
                }
            });
            
            Messenger.Register<BudgetMonthCategoryGroupViewModel, BudgetedAmountResetMessage>(this, (r, m) =>
            {
                if (r._budgetMonth == m.Month)
                {
                    r.OnPropertyChanged(nameof(TotalBudgeted));
                }
                r.OnPropertyChanged(nameof(TotalBalance));
            });
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            foreach (var category in Categories)
            {
                category.IsActive = false;
            }
        }
    }
}
