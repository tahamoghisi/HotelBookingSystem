using HotelBooking.Application.Common.Models;
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

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllAsync();
            return Ok(rooms);
        }
        //صفخه بندی و تعداد کل و مرتب سازی
        [HttpGet("search")]
        public async Task<IActionResult> Search(int? roomNumber, RoomStatus? status, PaginationRequest pagination, SortingRequest sorting)
        {
            var result = await _roomService.SearchPagedAsync(roomNumber, status, pagination, sorting);

            return Ok(result);
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int hotelId,  int page = 1, int pageSize = 10)
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
    }
}
