using HotelBooking.Application.DTOs.Booking;
using HotelBooking.Application.Mapping.BookingMap;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Infrastructure.Service;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Domain.Entities.Booking;
using static HotelBooking.Domain.Entities.Room;

namespace HotelBooking.Tests
{
    public class BookingServiceTests
    {
        [Fact]
        public async Task GetByIdForUserAsync_Should_Return_Null_When_Booking_Belongs_To_Another_User()
        {
            var customer = new Customer
            {
                Id = 1,
                UserId = 1,
                NationalCode = "123",
                FullName = "Ali",
            };
            var booking = new Booking
            {
                Id = 1,
                CustomerId = 2,
            };
            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();
            unitOfWork.Setup(x => x.Customers.GetByUserIdAsync(1)).ReturnsAsync(customer);
            unitOfWork.Setup(x => x.Bookings.GetByIdAsync(1)).ReturnsAsync(booking);
            var service = new BookingService(unitOfWork.Object, logger.Object);

            var result = await service.GetByIdForUserAsync(1,1);
            Assert.Null(result);
        }
        [Fact]
        public async Task CheckOutAsync_Should_Set_Room_Status_To_Reserved_When_Current_Booking_Exists()
        {
            var booking = new Booking
            {
                Id = 1,
                RoomId = 10,
                Status = BookingStatus.CheckedIn,
                CheckInDate = DateTime.Now.Date,
                CheckOutDate = DateTime.Now.Date.AddDays(2)
            };

            var room = new Room
            {
                Id = 10,
                Status = RoomStatus.Occupied
            };
            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();
            unitOfWork.Setup(x => x.Bookings.GetByIdAsync(1)).ReturnsAsync(booking);

            unitOfWork.Setup(x => x.Rooms.GetByIdAsync(10)).ReturnsAsync(room);

            unitOfWork.Setup(x => x.Bookings.HasCurrentBookingAsync(10, 1)).ReturnsAsync(true);
            var service = new BookingService(unitOfWork.Object, logger.Object);

            var result = await service.CheckOutAsync(1);
            Assert.True(result);

            Assert.Equal(RoomStatus.Reserved, room.Status);
        }
        [Fact]
        public async Task CheckOutAsync_Should_Set_Room_Status_To_Available_When_No_Current_Booking_Exists()
        {
            var booking = new Booking
            {
                Id = 1,
                RoomId = 10,
                Status = BookingStatus.CheckedIn,
                CheckInDate = DateTime.Now.Date,
                CheckOutDate = DateTime.Now.Date.AddDays(2)
            };

            var room = new Room
            {
                Id = 10,
                Status = RoomStatus.Occupied
            };
            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();
            unitOfWork.Setup(x => x.Bookings.GetByIdAsync(1)).ReturnsAsync(booking);

            unitOfWork.Setup(x => x.Rooms.GetByIdAsync(10)).ReturnsAsync(room);

            unitOfWork.Setup(x => x.Bookings.HasCurrentBookingAsync(10,1)).ReturnsAsync(false);
            var service = new BookingService(unitOfWork.Object, logger.Object);

            var result = await service.CheckOutAsync(1);
            Assert.True(result);

            Assert.Equal(RoomStatus.Available,room.Status);
        }
        [Fact]
        public async Task CheckOutAsync_Should_Return_False_When_Booking_NotFound()
        {
            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();

            unitOfWork.Setup(x => x.Bookings.GetByIdAsync(1)).ReturnsAsync((Booking?)null);

            var service = new BookingService(unitOfWork.Object, logger.Object);

            var result = await service.CheckOutAsync(1);

            Assert.False(result);

            unitOfWork.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }
        [Fact]
        public async Task CheckOutAsync_Should_Throw_When_Booking_IsNot_CheckIn()
        {
            var booking = new Booking
            {
                Id = 1,
                RoomId = 10,
                Status = BookingStatus.Confirmed,
                CheckInDate = DateTime.Now.Date,
                CheckOutDate = DateTime.Now.Date.AddDays(2)
            };
            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();

            unitOfWork.Setup(x => x.Bookings.GetByIdAsync(1)).ReturnsAsync(booking);

            var service = new BookingService(unitOfWork.Object, logger.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CheckOutAsync(1));
        }



        [Fact]
        public async Task CheckInAsync_Should_Throw_When_CheckInDate_Is_Bigger_Than_Now()
        {
            var booking = new Booking
            {
                Id = 1,
                RoomId = 10,
                Status = BookingStatus.Confirmed,
                CheckInDate = DateTime.Now.Date.AddDays(1),
                CheckOutDate = DateTime.Now.Date.AddDays(2)
            };

            var room = new Room
            {
                Id = 10,
                Status = RoomStatus.Available
            };
            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();

            unitOfWork.Setup(x => x.Bookings.GetByIdAsync(1)).ReturnsAsync(booking);
            unitOfWork.Setup(x => x.Rooms.GetByIdAsync(10)).ReturnsAsync(room);

            var service = new BookingService(unitOfWork.Object, logger.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CheckInAsync(1));

        }


        [Fact]
        public async Task ConfirmAsync_Should_Confirm_Pending_Booking()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                RoomId = 10,
                Status = BookingStatus.Pending,
                CheckInDate = DateTime.Now.Date,
                CheckOutDate = DateTime.Now.Date.AddDays(2)
            };

            var room = new Room
            {
                Id = 10,
                Status = RoomStatus.Available
            };
            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();


            unitOfWork.Setup(x => x.Bookings.GetByIdAsync(1))
                .ReturnsAsync(booking);
            unitOfWork.Setup(x => x.Rooms.GetByIdAsync(10))
                .ReturnsAsync(room);
            unitOfWork.Setup(x => x.Bookings.IsRoomAvailableAsync(
                    10,
                    booking.CheckInDate,
                    booking.CheckOutDate,
                    1))
                .ReturnsAsync(true);
            var service = new BookingService(unitOfWork.Object, logger.Object);
            // Act
            var result = await service.ConfirmAsync(1);


            // Assert
            Assert.True(result);
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
            Assert.Equal(RoomStatus.Reserved, room.Status);


            unitOfWork.Verify(x => x.Bookings.Update(booking), Times.Once);

            unitOfWork.Verify(x => x.Rooms.Update(room), Times.Once);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelAsync_Should_Cancel_Confirmed_Booking()
        {
            var booking = new Booking
            {
                Id = 1,
                RoomId = 10,
                Status = BookingStatus.Confirmed
            };
            var room = new Room
            {
                Id = 10,
                Status = RoomStatus.Reserved
            };
            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();


            unitOfWork.Setup(x => x.Bookings.GetByIdAsync(1))
                .ReturnsAsync(booking);
            unitOfWork.Setup(x => x.Rooms.GetByIdAsync(10))
                .ReturnsAsync(room);

            var service = new BookingService(unitOfWork.Object, logger.Object);

            var result = await service.CancelAsync(1);

            Assert.True(result);
            Assert.Equal(BookingStatus.Cancelled, booking.Status);
            Assert.Equal(RoomStatus.Available, room.Status);
        }

        [Fact]
        public async Task CancelAsync_Should_Throw_When_Booking_Is_Completed()
        {
            var booking = new Booking
            {
                Id = 1,
                RoomId = 10,
                Status = BookingStatus.Completed
            };

            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();


            unitOfWork.Setup(x => x.Bookings.GetByIdAsync(1))
                .ReturnsAsync(booking);

            var service = new BookingService(unitOfWork.Object, logger.Object);

            var result = await service.CancelAsync(1);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CancelAsync(1));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_CheckInDate_Is_After_CheckOutDate()
        {
            int userId = 2;
            var createBookingDto = new CreateBookingDTO
            {
                //CustomerId = 2,
                CheckInDate = new DateTime(2026, 9, 20),
                CheckOutDate = new DateTime(2026, 9, 18),
                RoomId = 2,
                HotelId = 2
            };
            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();


            unitOfWork.Setup(x => x.Rooms.GetByIdAsync(2))
                  .ReturnsAsync(new Room
                  {
                      Id = 2
                  });

            var service = new BookingService(unitOfWork.Object, logger.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(createBookingDto, userId));
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Booking_When_Data_Is_Valid()
        {
            int userId = 2;
            var createBookingDto = new CreateBookingDTO
            {
                //CustomerId = 2,
                CheckInDate = new DateTime(2026, 9, 20),
                CheckOutDate = new DateTime(2026, 9, 23),
                RoomId = 2,
                HotelId = 2
            };
            var room = new Room
            {
                Id = 2,
                PricePerNight = 100
            };
            var customer = new Customer
            {
                Id = 2,
                FullName = "Ali"
            };
            var hotel = new Hotel
            {
                Id = 2,
                Name = "Laleh"
            };


            var unitOfWork = new Mock<IUnitOFWork>();
            var logger = new Mock<ILogger<BookingService>>();


            unitOfWork.Setup(x => x.Rooms.GetByIdAsync(2))
                .ReturnsAsync(room);
            unitOfWork.Setup(x => x.Hotels.GetByIdAsync(2))
                .ReturnsAsync(hotel);
            unitOfWork.Setup(x => x.Customers.GetByUserIdAsync(userId))
                .ReturnsAsync(customer);
            unitOfWork.Setup(x => x.Rooms.IsRoomAvailableAsync(
                    2,
                    createBookingDto.CheckInDate,
                    createBookingDto.CheckOutDate))
                .ReturnsAsync(true);

            var service = new BookingService(unitOfWork.Object, logger.Object);

            var result = await service.CreateAsync(createBookingDto, userId);

            Assert.Equal(BookingStatus.Pending, result.Status);
            Assert.Equal(300, result.TotalPrice);
            Assert.Equal(customer.Id, result.CustomerId);

        }
    }
}
