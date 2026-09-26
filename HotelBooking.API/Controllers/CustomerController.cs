using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Customer;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }
        //صفخه بندی و تعداد کل و مرتب سازی
        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? fullName, [FromQuery] string? email, [FromQuery] string? nationalCode, [FromQuery] PaginationRequest pagination, [FromQuery] SortingRequest sorting)
        {
            var result = await _customerService.SearchPagedAsync(fullName, email, nationalCode, pagination, sorting);

            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,UpdateCustomerDTO dto)
        {
            var result = await _customerService.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> ME()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var customer = await _customerService.GetByUserIdAsync(userId);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateME(UpdateCustomerDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var customer = await _customerService.GetByUserIdAsync(userId);

            if (customer == null)
                return NotFound();
            await _customerService.UpdateAsync(customer.Id, dto);

            var updatedCustomer = await _customerService.GetByUserIdAsync(userId);


            return Ok(updatedCustomer);
        }
    }
}
