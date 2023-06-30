using Earmark.Backend.Models;

namespace Earmark.Backend.Services
{
    public interface IAccountDetailService
    {
        decimal GetTotalActivityForMonth(int month, int year);

        decimal GetTotalActivityForMonth(int month, int year, CategoryGroup categoryGroup);

        decimal GetTotalActivityForMonth(int month, int year, Category category);

        decimal GetTotalIncomeForMonth(int month, int year);
    }
}
