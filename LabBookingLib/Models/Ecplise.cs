namespace LabBookingLib.Models;
using LabBookingLib.BookingSystem;
using LabBookingLib.ApiClients;


public class Eclipse : IInstrument
{
    public IBookingSystem BookingAPI { get; private set; }
    public Eclipse(IBookingSystem bookingSystem)
    {
        BookingAPI = bookingSystem;
    }
    public Eclipse() : this(new QReserve(new QReserveApiClient())) { }


    public string FileBackupLocation { get; } = "\\network\\location";
    public string ResourceIdentifier { get; } = QReserveConstants.EclipseIdentifier;



    public Task<bool> MakeBookingAsync(string projectCode, DateTime startTime, double bookingDuration)
    {

        return BookingAPI.MakeBookingAsync(projectCode, startTime, bookingDuration, ResourceIdentifier);
    }
}