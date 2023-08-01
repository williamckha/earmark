using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Earmark.WebApi.Controllers
{
    [ApiController]
    [Route("api/payees")]
    public class PayeeController : ControllerBase
    {
        private IPayeeService _payeeService;

        public PayeeController(IPayeeService payeeService)
        {
            _payeeService = payeeService;
        }

        [HttpGet("")]
        public IEnumerable<PayeeDTO> GetPayees()
        {
            return _payeeService.GetPayees().Select(x => PayeeToDTO(x));
        }


        [HttpGet("{id:int}")]
        public ActionResult<PayeeDTO> GetPayee(int id)
        {
            var payee = _payeeService.GetPayees().FirstOrDefault(x => x.Id == id);

            if (payee is null)
            {
                return NotFound();
            }

            return PayeeToDTO(payee);
        }

        [HttpPost("")]
        public ActionResult<PayeeDTO> AddPayee(PayeeDTO payee)
        {
            try
            {
                var createdPayee = _payeeService.AddPayee(payee.Name);
                var payeeDTO = PayeeToDTO(createdPayee);
                return CreatedAtAction(nameof(GetPayee), new { id = payeeDTO.Id }, payeeDTO);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeletePayee(int id)
        {
            try
            {
                _payeeService.RemovePayee(id);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        private PayeeDTO PayeeToDTO(Payee payee)
        {
            return new PayeeDTO()
            {
                Id = payee.Id,
                Name = payee.Name
            };
        }
    }
}
