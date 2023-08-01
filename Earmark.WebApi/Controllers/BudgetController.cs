using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Earmark.WebApi.Controllers
{
    [ApiController]
    [Route("api/budget")]
    public class BudgetController : ControllerBase
    {
        private IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet("months")]
        public IEnumerable<BudgetMonthDTO> GetBudgetMonths()
        {
            return _budgetService.GetBudgetMonths().Select(x => BudgetMonthToDTO(x));
        }
        
        [HttpGet("months/{id:int}")]
        public ActionResult<BudgetMonthDTO> GetBudgetMonth(int id)
        {
            var budgetMonth = _budgetService
                .GetBudgetMonths()
                .FirstOrDefault(x => x.Id == id);

            if (budgetMonth is null)
            {
                return NotFound();
            }

            return BudgetMonthToDTO(budgetMonth);
        }

        [HttpPost("months")]
        public ActionResult<BudgetMonthDTO> AddBudgetMonth(BudgetMonthDTO budgetMonth)
        {
            try
            {
                var createdBudgetMonth = _budgetService.AddBudgetMonth(budgetMonth.Month, budgetMonth.Year);
                var budgetMonthDTO = BudgetMonthToDTO(createdBudgetMonth);
                return CreatedAtAction(nameof(GetBudgetMonth), new { id = budgetMonthDTO.Id }, budgetMonthDTO);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }
 
        private BudgetMonthDTO BudgetMonthToDTO(BudgetMonth budgetMonth)
        {
            return new BudgetMonthDTO()
            {
                Id = budgetMonth.Id,
                Month = budgetMonth.Month,
                Year = budgetMonth.Year,
                TotalUnbudgeted = budgetMonth.TotalUnbudgeted
            };
        }
    }
}
