namespace LabBooking.Tests.Models;
using LabBookingLib.Models;
using LabBookingLib.BookingSystem;
using Moq;

public class Eclipse_Should
{
    [Fact]
    public async Task MakeBookingAsync_CallsIBookingSystemWithCorrectArguments()
    {
        // Arrange
        var mockBookingAPI = new Mock<IBookingSystem>();
        var service = new Eclipse(mockBookingAPI.Object);
        var projectCode = "project123";
        var startTime = DateTime.Now;
        var bookingDuration = 2.5;

        // Act
        await service.MakeBookingAsync(projectCode, startTime, bookingDuration);

        // Assert
        mockBookingAPI.Verify(api => api.MakeBookingAsync(
            projectCode,
            startTime,
            bookingDuration,
            service.ResourceIdentifier),
            Times.Once);
    }
    [Fact]
    public async Task MakeBookingAsync_ReturnsTrue()
    {
        // Arrange
        var mockBookingAPI = new Mock<IBookingSystem>();
        mockBookingAPI.Setup(api => api.MakeBookingAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<string>()))
                                                        .ReturnsAsync(true);
        var service = new Eclipse(mockBookingAPI.Object);
        var projectCode = "project123";
        var startTime = DateTime.Now;
        var bookingDuration = 2.5;

        // Act
        bool result = await service.MakeBookingAsync(projectCode, startTime, bookingDuration);

        // Assert
        Assert.True(result);
    }
}

