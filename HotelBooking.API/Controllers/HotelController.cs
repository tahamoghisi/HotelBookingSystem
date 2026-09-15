using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.ServiceInterface;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id);

            if (hotel == null)
                return NotFound();

            return Ok(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHotelDTO dto)
        {
            var hotel = await _hotelService.CreateAsync(dto);
            return Ok(hotel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateHotelDTO dto)
        {
            var result = await _hotelService.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

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
