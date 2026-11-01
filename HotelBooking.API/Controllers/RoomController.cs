using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Image;
using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static HotelBooking.Domain.Entities.Room;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;

        public RoomController(IRoomService roomService, IBookingService bookingService)
        {
            _roomService = roomService;
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllAsync();
            return Ok(rooms);
        }
        //صفخه بندی و تعداد کل و مرتب سازی
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int? hotelId, [FromQuery] int? roomNumber, [FromQuery] RoomStatus? status, [FromQuery] int? minPrice, [FromQuery] int? maxPrice, [FromQuery] PaginationRequest pagination, [FromQuery] SortingRequest sorting)
        {
            var result = await _roomService.SearchPagedAsync(hotelId, roomNumber, status, minPrice, maxPrice, pagination, sorting);

            return Ok(result);
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int hotelId, int page = 1, int pageSize = 10)
        {
            if (page < 1)
                return BadRequest("Page must be greater than 0.");

            if (pageSize < 1)
                return BadRequest("PageSize must be greater than 0.");
            if (pageSize > 100)
                return BadRequest("PageSize cannot be greater than 100.");
            var result = await _roomService.GetPagedAsync(hotelId, page, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _roomService.GetByIdAsync(id);

            if (room == null)
                return NotFound();

            return Ok(room);
        }
        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetHotelRooms(int hotelId)
        {
            var rooms = await _roomService.GetByHotelIdAsync(hotelId);
            
            if (rooms == null)
                return NotFound();

            return Ok(rooms);
        }

        [HttpGet("{id}/availability")]
        public async Task<IActionResult> CheckAvailability(int id, [FromQuery] DateTime checkInDate, [FromQuery] DateTime checkOutDate)
        {
            if (checkInDate >= checkOutDate)
                return BadRequest("تاریخ خروج باید بعد از تاریخ ورود باشد.");

            var room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return NotFound();

            var available = room.Status != RoomStatus.Maintenance
                && await _bookingService.IsRoomAvailableAsync(id, checkInDate, checkOutDate);

            return Ok(new { isAvailable = available });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoomDTo dto)
        {
            var room = await _roomService.CreateAsync(dto);
            return Ok(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateRoomDTO dto)
        {
            var result = await _roomService.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roomService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/maintenance")]
        public async Task<IActionResult> SetMaintenance(int id)
        {
            var result = await _roomService.SetMaintenanceAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/available")]
        public async Task<IActionResult> SetAvailable(int id)
        {
            var result = await _roomService.SetAvailableAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
        [HttpPost("{roomId}/images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRoomImages(int roomId, [FromForm] List<IFormFile> images)
        {
            var result = await _roomService.AddRoomImagesAsync(roomId, images);

            return Ok(ApiResponse<List<RoomImageResponseDTO>>.Ok(result));
        }
        [HttpDelete("{roomId}/images/{imageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoomImage(int roomId, int imageId)
        {
            var result = await _roomService.DeleteRoomImageAsync(roomId, imageId);

            if (!result)
                return NotFound(ApiResponse<string>.Fail("Room image not found."));

            return Ok(ApiResponse<string>.Ok(
                "Room image deleted successfully."));
        }
        [HttpPut("{roomId}/images/{imageId}/main")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetMainRoomImage(int roomId, int imageId)
        {
            var result = await _roomService.SetMainRoomImageAsync(roomId, imageId);

            if (!result)
                return NotFound(ApiResponse<string>.Fail("Room image not found."));

            return Ok(ApiResponse<string>.Ok(
                "Room image set as main successfully."));
        }
    }
}
