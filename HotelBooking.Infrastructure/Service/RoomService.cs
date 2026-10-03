using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.DTOs.Image;
using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.Mapping.HotelMap;
using HotelBooking.Application.Mapping.RoomMap;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Entities.Images;
using HotelBooking.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Room;

namespace HotelBooking.Infrastructure.Service
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOFWork _unitOFWork;
        private readonly ILogger<RoomService> _logger;
        public RoomService(IUnitOFWork unitOFWork, ILogger<RoomService> logger)
        {
            _logger = logger;
            _unitOFWork = unitOFWork;
        }
        public async Task<RoomResponseDTO> CreateAsync(CreateRoomDTo dto)
        {
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(dto.HotelId);

            if (hotel == null)
            {
                _logger.LogWarning("hotel {HotelId} not found for roomNumber {roomNumber}", dto.HotelId,dto.RoomNumber);
                throw new ArgumentException("Hotel Not Found!");
            }

            // ساخت Room
            var room = RoomMapping.ToEntity(dto);
            room.Status = RoomStatus.Available;

            await _unitOFWork.Rooms.AddAsync(room);
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("Room {RoomId} created successfully.", room.Id);

            return RoomMapping.ToDto(room);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _unitOFWork.Rooms.GetByIdAsync(id);
            if (room == null) return false;
            if (room.Status == RoomStatus.Occupied)
            {
                _logger.LogWarning("cannot delete occupied room {roomId}", room.Id);
                throw new InvalidOperationException("Cannot delete an occupied room.");
            }
                

            if (room.Status == RoomStatus.Reserved)
            {
                _logger.LogWarning("cannot delete reserved room {roomId}", room.Id);
                throw new InvalidOperationException(
                    "Cannot delete a reserved room.");
            }
                

            var hasBookings = await _unitOFWork.Bookings
                .HasActiveBookingsAsync(id);

            if (hasBookings)
            {
                _logger.LogWarning("Cannot delete Room {RoomId} because it has active bookings.", room.Id);
                throw new InvalidOperationException(
                    "Cannot delete a room with active bookings.");
            }
                
            _unitOFWork.Rooms.Remove(room);
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("Room {RoomId} deleted successfully.", room.Id);

            return true;
        }

        public async Task<IEnumerable<RoomResponseDTO>> GetAllAsync()
        {
            var rooms = await _unitOFWork.Rooms.GetAllRoomsAsync();
            return rooms.Select(x => RoomMapping.ToDto(x)).ToList();
        }

        public async Task<IEnumerable<RoomResponseDTO>> GetByHotelIdAsync(int hotelId)
        {
            var rooms = await _unitOFWork.Rooms.GetByHotelIdAsync(hotelId);
            return rooms.Select(x => RoomMapping.ToDto(x)).ToList();
        }

        public async Task<RoomResponseDTO?> GetByIdAsync(int id)
        {
            var room = await _unitOFWork.Rooms.GetByRoomIdAsync(id);

            if (room == null)
                return null;

            return RoomMapping.ToDto(room);
        }

        public async Task<bool> UpdateAsync(int id, UpdateRoomDTO dto)
        {
            var room = await _unitOFWork.Rooms.GetByIdAsync(id);
            if (room == null) return false;
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(dto.HotelId);
            if (hotel == null)
            {
                _logger.LogWarning("Hotel {HotelId} not found while updating Room {RoomNumber}.", dto.HotelId, dto.RoomNumber);
                throw new ArgumentException("Hotel Not Found!");
            }
            room.HotelId = dto.HotelId;
            room.RoomNumber = dto.RoomNumber;
            room.Type = dto.Type;
            room.PricePerNight = dto.PricePerNight;
            room.Capacity = dto.Capacity;

            _unitOFWork.Rooms.Update(room);

            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("Room {RoomId} updated successfully.", room.Id);


            return true;
        }
        public async Task<PagedResult<RoomResponseDTO>> GetPagedAsync(int hotelId, int page, int pageSize)
        {
            var totalCount = await _unitOFWork.Rooms.CountByHotelAsync(hotelId);
            var pageItems = await _unitOFWork.Rooms.GetPagedByHotelAsync(hotelId, page, pageSize);
            var dto = pageItems.Select(x => RoomMapping.ToDto(x)).ToList();
            return new PagedResult<RoomResponseDTO>
            {
                Items = dto,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        //صفخه بندی و تعداد کل و مرتب سازی
        public async Task<PagedResult<RoomResponseDTO>> SearchPagedAsync(int? hotelId, int? roomNumber, RoomStatus? status, int? MinPrice, int? maxPrice, PaginationRequest pagination, SortingRequest sorting)
        {
            var pageItems = await _unitOFWork.Rooms.SearchPagedAsync(hotelId, roomNumber, status, MinPrice, maxPrice, pagination.Page, pagination.PageSize, sorting.SortBy, sorting.Descending);
            var dto = pageItems.Items.Select(r => RoomMapping.ToDto(r)).ToList();
            return new PagedResult<RoomResponseDTO>
            {
                Items = dto,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = pageItems.TotalCount
            };

        }
        public async Task<List<RoomImageResponseDTO>> AddRoomImagesAsync(int roomId, List<IFormFile> images)
        {
            var room = await _unitOFWork.Rooms.GetByRoomIdAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException("Hotel not found.");

            if (images == null || images.Count == 0)
                throw new ArgumentException("At least one image is required.");

            var roomImages = new List<RoomImage>();
            var hasMainImage = await _unitOFWork.RoomImage.HasMainImageAsync(roomId);

            foreach (var image in images)
            {
                if (image.Length == 0)
                    continue;

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                var folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "rooms",
                    roomId.ToString());

                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(
                    filePath,
                    FileMode.Create);

                await image.CopyToAsync(stream);

                var roomImage = new RoomImage
                {
                    RoomId = roomId,
                    ImageUrl = $"/images/rooms/{roomId}/{fileName}",
                    IsMain = !hasMainImage && roomImages.Count == 0
                };

                roomImages.Add(roomImage);
            }

            await _unitOFWork.RoomImage.AddRangeAsync(roomImages);

            await _unitOFWork.SaveChangesAsync();

            return roomImages.Select(x => new RoomImageResponseDTO
            {
                Id = x.Id,
                ImageUrl = x.ImageUrl,
                IsMain = x.IsMain
            }).ToList();
        }
        #region MaintenanceStatus
        public async Task<bool> SetMaintenanceAsync(int roomId)
        {
            var room = await _unitOFWork.Rooms.GetByIdAsync(roomId);

            if (room == null)
                return false;

            if (room.Status == RoomStatus.Occupied ||
                room.Status == RoomStatus.Reserved)
            {
                _logger.LogWarning("Cannot put Room {RoomId} into maintenance because it is occupied or reserved.", room.Id);
                throw new InvalidOperationException(
                    "Cannot put a reserved or occupied room into maintenance.");
            }

            room.Status = RoomStatus.Maintenance;

            _unitOFWork.Rooms.Update(room);
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("Room {RoomId} set to maintenance successfully.", room.Id);

            return true;
        }
        public async Task<bool> SetAvailableAsync(int roomId)
        {
            var room = await _unitOFWork.Rooms.GetByIdAsync(roomId);

            if (room == null)
                return false;

            if (room.Status != RoomStatus.Maintenance)
            {
                _logger.LogWarning("Cannot set Room {RoomId} to available because it is not under maintenance.", room.Id);
                throw new InvalidOperationException(
                    "Room is not under maintenance.");
            }
                

            room.Status = RoomStatus.Available;

            _unitOFWork.Rooms.Update(room);
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("Room {RoomId} set to available successfully.", room.Id);

            return true;
        }


        #endregion
    }
}
