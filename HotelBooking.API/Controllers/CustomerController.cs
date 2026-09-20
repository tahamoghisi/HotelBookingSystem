using HotelBooking.Application.DTOs.Customer;
using HotelBooking.Application.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateCustomerDTO dto)
        //{
        //    var customer = await _customerService.CreateAsync(dto);
        //    return Ok(customer);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateCustomerDTO dto)
        {
            var result = await _customerService.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
