using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static HotelBooking.Domain.Entities.Booking;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int page = 1,int pageSize = 10,int? hotelId = null,int? customerId = null,int? roomId = null,BookingStatus? status = null)
        {
            var result = await _bookingService.GetPagedAsync(page,pageSize, hotelId, customerId, roomId, status);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingDTO dto)
        {
            var booking = await _bookingService.CreateAsync(dto);

            return Ok(booking);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,UpdateBookingDTO dto){
            var result = await _bookingService.UpdateAsync(id, dto);
            if (!result)return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _bookingService.CancelAsync(id);
            if (!result)return NotFound();
            return NoContent();
        }
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _bookingService.ConfirmAsync(id);
            if (!result)return NotFound();
            return NoContent();
        }
        [HttpPut("{id}/check-in")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var result = await _bookingService.CheckInAsync(id);
            if (!result)return NotFound();
            return NoContent();
        }
        [HttpPut("{id}/check-out")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var result = await _bookingService.CheckOutAsync(id);
            if (!result)return NotFound();
            return NoContent();
        }
    }
}
