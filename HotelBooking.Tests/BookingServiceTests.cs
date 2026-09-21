using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Infrastructure.Service;
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
        public async Task ConfirmAsync_Should_Confirm_Pending_Booking()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                RoomId = 10,
                Status = BookingStatus.Pending
            };

            var room = new Room
            {
                Id = 10,
                Status = RoomStatus.Available
            };

            var unitOfWork = new Mock<IUnitOFWork>();

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

            var service = new BookingService(unitOfWork.Object);


            // Act
            var result = await service.ConfirmAsync(1);


            // Assert
            Assert.True(result);
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
            Assert.Equal(RoomStatus.Reserved, room.Status);
        }
    }
}
