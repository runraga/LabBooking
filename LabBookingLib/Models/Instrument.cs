namespace LabBookingLib.Models;
using LabBookingLib.BookingSystem;



public interface IInstrument
{
    string FileBackupLocation { get; }
    string ResourceIdentifier { get; }
    IBookingSystem BookingAPI { get; }
    Task<bool> MakeBookingAsync(string projectCode, DateTime startTime, double bookingDuration);

}