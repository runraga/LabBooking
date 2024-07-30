namespace LabBookingLib.Models;



public interface IBooking
{
    void AddDataFile(IDatafile rawDataFile);
    DateTime GetBookingStartTime();
    int BookingLengthMinutes { get; }
    bool IsValidBooking { get; }
    bool IsWithinBookingWindow(IDatafile otherFile);
    bool BookingComplete { get; set; }
    string BookingError { get; }
    Task<bool> MakeBookingAsync();
    string ProjectCode { get; }

    //Dictionary<string, List<string>> GetDataFileErrors();
    List<string> GetDataFileNames();
    public IInstrument Instrument { get; }

}
public class BookingException : Exception
{
    public BookingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
