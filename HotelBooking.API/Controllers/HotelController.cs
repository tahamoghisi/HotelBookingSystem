using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var hotels = await _hotelService.GetAllAsync();
            return Ok(hotels);
        }
        //صفخه بندی و تعداد کل و مرتب سازی
        [HttpGet("search")]
        public async Task<IActionResult> Search(string? name, string? city, PaginationRequest pagination, SortingRequest sorting)
        {
            var result = await _hotelService.SearchPagedAsync(name, city, pagination, sorting);

            return Ok(result);
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
        {
            if (page < 1)
                return BadRequest("Page must be greater than 0.");

            if (pageSize < 1)
                return BadRequest("PageSize must be greater than 0.");
            if (pageSize > 100)
                return BadRequest("PageSize cannot be greater than 100.");
            var result = await _hotelService.GetPagedAsync(page, pageSize);

            return Ok(result);
        }
        [HttpGet("{hotelId}/rooms")]
        public async Task<IActionResult> GetHotelRooms(int hotelId)
        {
            var rooms = await _hotelService.GetHotelRoomsAsync(hotelId);

            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id);

            if (hotel == null)
                return NotFound();

            return Ok(hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateHotelDTO dto)
        {
            var hotel = await _hotelService.CreateAsync(dto);
            return Ok(hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateHotelDTO dto)
        {
            var result = await _hotelService.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _hotelService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
