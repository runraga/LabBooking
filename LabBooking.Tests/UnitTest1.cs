using LabBookingLib;
namespace LabBooking.Tests;

public class UnitTest1
{
    [Fact]
    public void Add_ShouldReturnCorrectSum()
    {
        MathUtilities m = new();
        int result = m.Add(1, 2);
        Assert.Equal(3, result);
    }
}