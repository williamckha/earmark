using Earmark.Backend.Helpers;
using Earmark.Converters;
using Earmark.Helpers;
using Earmark.ViewModels.Budget;
using Microsoft.UI.Xaml.Controls;

namespace Earmark.UserControls.Budget
{
    public sealed partial class BudgetMonthHeader : UserControl
    {
        public BudgetMonthViewModel ViewModel => DataContext as BudgetMonthViewModel;

        public BudgetMonthHeader()
        {
            this.InitializeComponent();
        }

        private string GetUnbudgetedLastMonthLabel(int unbudgetedLastMonth)
        {
            (int lastMonth, _) = DateTimeHelper.GetPreviousMonth(ViewModel.Month, ViewModel.Year);
            if (unbudgetedLastMonth < 0)
            {
                return string.Format(
                    "MonthSummaryLabel_OverbudgetedLastMonth".GetLocalizedResource(),
                    DateTimeConverter.MonthNumberToAbbreviation(lastMonth, false));
            }
            else
            {
                return string.Format(
                    "MonthSummaryLabel_UnbudgetedLastMonth".GetLocalizedResource(),
                    DateTimeConverter.MonthNumberToAbbreviation(lastMonth, false));
            }
        }

        private string GetOverspentLastMonthLabel()
        {
            (int lastMonth, _) = DateTimeHelper.GetPreviousMonth(ViewModel.Month, ViewModel.Year);
            return string.Format(
                "MonthSummaryLabel_OverspentLastMonth".GetLocalizedResource(),
                DateTimeConverter.MonthNumberToAbbreviation(lastMonth, false));
        }

        private string GetIncomeThisMonthLabel()
        {
            return string.Format(
                "MonthSummaryLabel_IncomeThisMonth".GetLocalizedResource(),
                DateTimeConverter.MonthNumberToAbbreviation(ViewModel.Month, false));
        }

        private string GetBudgetedThisMonthLabel()
        {
            return string.Format(
                "MonthSummaryLabel_BudgetedThisMonth".GetLocalizedResource(),
                DateTimeConverter.MonthNumberToAbbreviation(ViewModel.Month, false));
        }

        private string GetTotalUnbudgetedLabel(int totalUnbudgeted)
        {
            return (totalUnbudgeted < 0) ? 
                "OverbudgetedThisMonthLabel".GetLocalizedResource() :
                "ToBudgetThisMonthLabel".GetLocalizedResource();
        }
    }
}
