using LabBookingLib.ApiResponses;



namespace LabBookingLib.BookingSystem
{
    public interface IBookingSystem
    {
        Task<bool> MakeBookingAsync(string projectCode, DateTime startTime, double bookingDurationMinutes, string resourceIdentifier);


    }

}