using HotelBooking.Application.Common.Models;
using HotelBooking.Application.DTOs.Customer;
using HotelBooking.Application.DTOs.Hotel;
using HotelBooking.Application.DTOs.Image;
using HotelBooking.Application.DTOs.Room;
using HotelBooking.Application.Mapping.CustomerMap;
using HotelBooking.Application.Mapping.HotelMap;
using HotelBooking.Application.Mapping.RoomMap;
using HotelBooking.Application.ServiceInterface;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Entities.Images;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HotelBooking.Infrastructure.Service
{
    public class HotelService : IHotelService
    {
        private readonly IUnitOFWork _unitOFWork;
        private readonly ILogger<HotelService> _logger;
        public HotelService(IUnitOFWork unitOFWork, ILogger<HotelService> logger)
        {
            _logger = logger;
            _unitOFWork = unitOFWork;
        }
        public async Task<HotelResponseDTO> CreateAsync(CreateHotelDTO dto)
        {
            var hotel = HotelMapping.ToEntity(dto);

            await _unitOFWork.Hotels.AddAsync(hotel);
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("hotel {HotelId} created successfully", hotel.Id);

            return HotelMapping.ToDto(hotel);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(id);
            if (hotel == null) return false;
            var hasActiveRooms = await _unitOFWork.Hotels.HasActiveRoomsAsync(hotel.Id);
            if (hasActiveRooms)
            {
                _logger.LogWarning("Cannot delete hotel {HotelId} because it has active rooms.", hotel.Id);
                throw new InvalidOperationException("Cannot delete a hotel with reserved or occupied rooms.");
            }
            _unitOFWork.Hotels.Remove(hotel);
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("hotel {HotelId} deleted successfully.", hotel.Id);
            return true;
        }

        public async Task<IEnumerable<HotelResponseDTO>> GetAllAsync()
        {
            var hotels = await _unitOFWork.Hotels.GetAllHotelsAsync();
            return hotels.Select(x => HotelMapping.ToDto(x)).ToList();
        }

        public async Task<HotelResponseDTO?> GetByIdAsync(int id)
        {
            var hotel = await _unitOFWork.Hotels.GetByHotelIdAsync(id);
            if (hotel == null)
            {
                return null;
            }
            return HotelMapping.ToDto(hotel);
        }

        public async Task<bool> UpdateAsync(int id, UpdateHotelDTO dto)
        {
            var hotel = await _unitOFWork.Hotels.GetByIdAsync(id);
            if (hotel == null)
            {
                return false;
            }
            hotel.Name = dto.Name;
            hotel.City = dto.City;
            hotel.Country = dto.Country;
            hotel.Address = dto.Address;
            hotel.Description = dto.Description;
            hotel.Email = dto.Email;
            hotel.StarRating = dto.StarRating;
            hotel.PhoneNumber = dto.PhoneNumber;
            _unitOFWork.Hotels.Update(hotel);
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("hotel {HotelId} updated successfully", hotel.Id);
            return true;
        }
        public async Task<IEnumerable<RoomResponseDTO>> GetHotelRoomsAsync(int hotelId)
        {
            var hotel = await _unitOFWork.Hotels.GetByHotelIdAsync(hotelId);
            if (hotel == null)
            {
                throw new InvalidOperationException("Hotel not found!");
            }
            var hotelsRooms = await _unitOFWork.Rooms.GetByHotelIdAsync(hotelId);
            return hotelsRooms.Select(x => RoomMapping.ToDto(x)).ToList();
        }

        public async Task<PagedResult<HotelResponseDTO>> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _unitOFWork.Hotels.GetCountAsync();
            var pageItems = await _unitOFWork.Hotels.GetPagedAsync(page, pageSize);
            var dto = pageItems.Select(x => HotelMapping.ToDto(x)).ToList();
            return new PagedResult<HotelResponseDTO>
            {
                Items = dto,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        //صفخه بندی و تعداد کل و مرتب سازی
        public async Task<PagedResult<HotelResponseDTO>> SearchPagedAsync(string? name, string? city, int? minStarRating, PaginationRequest pagination, SortingRequest sorting)
        {
            var pageItems = await _unitOFWork.Hotels.SearchPagedAsync(name, city, minStarRating, pagination.Page, pagination.PageSize, sorting.SortBy, sorting.Descending);
            var dto = pageItems.Items.Select(x => HotelMapping.ToDto(x)).ToList();
            return new PagedResult<HotelResponseDTO>
            {
                Items = dto,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = pageItems.TotalCount
            };
        }

        public async Task<List<HotelImageResponseDTO>> AddHotelImagesAsync(int hotelId, List<IFormFile> images)
        {
            var hotel = await _unitOFWork.Hotels.GetByHotelIdAsync(hotelId);

            if (hotel == null)
                throw new KeyNotFoundException("Hotel not found.");

            if (images == null || images.Count == 0)
                throw new ArgumentException("At least one image is required.");

            var hotelImages = new List<HotelImage>();
            var hasMainImage = await _unitOFWork.HotelImage.HasMainImageAsync(hotelId);

            foreach (var image in images)
            {
                if (image.Length == 0)
                    continue;

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                var folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "hotels",
                    hotelId.ToString());

                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(
                    filePath,
                    FileMode.Create);

                await image.CopyToAsync(stream);

                var hotelImage = new HotelImage
                {
                    HotelId = hotelId,
                    ImageUrl = $"/images/hotels/{hotelId}/{fileName}",
                    IsMain = !hasMainImage && hotelImages.Count == 0
                };

                hotelImages.Add(hotelImage);
            }

            await _unitOFWork.HotelImage.AddRangeAsync(hotelImages);

            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("hotel {HotelId} add hotel images successfully", hotelId);
            return hotelImages.Select(x => new HotelImageResponseDTO
            {
                Id = x.Id,
                ImageUrl = x.ImageUrl,
                IsMain = x.IsMain
            }).ToList();
        }

        public async Task<bool> DeleteHotelImageAsync(int hotelId, int imageId)
        {
            var image = await _unitOFWork.HotelImage.GetByIdAsync(hotelId, imageId);
            if (image == null)
            {
                _logger.LogWarning("hotel {HotelId} delete hotel image {ImageId} failed", hotelId, imageId);
                return false;
            }
            var wasMain = image.IsMain;
            var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            image.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await _unitOFWork.HotelImage.DeleteAsync(imageId);
            if (wasMain)
            {
                var firstImage = await _unitOFWork.HotelImage
                 .GetFirstImageAsync(hotelId);

                if (firstImage != null)
                {
                    firstImage.IsMain = true;
                }
            }
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("hotel {HotelId} delete hotel image {ImageId} successfully", hotelId, imageId);
            return true;
        }

        public async Task<bool> SetMainHotelImageAsync(int hotelId, int imageId)
        {
            var result = await _unitOFWork.HotelImage.SetMainImageAsync(hotelId, imageId);
            if (result == false)
            {
                _logger.LogWarning("hotel {HotelId} set main hotel image {ImageId} failed", hotelId, imageId);
                return false;
            }
            await _unitOFWork.SaveChangesAsync();
            _logger.LogInformation("hotel {HotelId} set main hotel image {ImageId} successfully", hotelId,imageId);
            return true;
        }
    }
}
