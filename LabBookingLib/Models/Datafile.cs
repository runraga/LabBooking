namespace LabBookingLib.Models;

public interface IDatafile
{
    List<string> Errors { get; }
    DateTime StartTime { get; }
    DateTime EndTime { get; }
    int DurationMinutes { get; }
    bool IsReadyToProcess { get; }

    string GetDataFileName();
}