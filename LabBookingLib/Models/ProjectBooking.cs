namespace LabBookingLib.Models;
public class ProjectBooking(string projectCode, IInstrument instrument) : IBooking
{
    public string BookingError { get; set; } = "";
    public bool BookingComplete { get; set; } = false;
    private readonly SortedList<DateTime, IDatafile> RawFiles = [];
    public string ProjectCode { get; } = projectCode;
    public IInstrument Instrument { get; } = instrument;
    public bool IsValidBooking
    {
        //TODO remove Error check once errored files aren't added to bookings
        get
        {

            return RawFiles.Count > 0 && BookingLengthMinutes > 15; // && !RawFilesHaveError();
        }
    }
    public async Task<bool> MakeBookingAsync()
    {
        try
        {
            bool success = await Instrument.MakeBookingAsync(ProjectCode, GetBookingStartTime(), BookingLengthMinutes);
            BookingError = "No booking errors found";
            BookingComplete = true;
            return true;
        }
        catch (Exception e)
        {
            BookingError = e.Message;
            return false;
        }
    }
    public int BookingLengthMinutes
    {
        get
        {
            if (RawFiles.Count == 0)
            {
                return 0;
            }

            IDatafile firstFile = RawFiles.Values.FirstOrDefault()!;

            IDatafile lastFile = RawFiles.Values.LastOrDefault()!;

            double duration = (lastFile.EndTime - firstFile.StartTime).TotalMinutes;

            return (int)Math.Ceiling(duration);
        }
    }
    public DateTime GetBookingStartTime()
    {
        if (RawFiles.Count == 0)
        {
            throw new InvalidOperationException("Not data files in booking, no start time available");
        }
        return RawFiles.Values[0].StartTime;
    }
    // TODO can remove once errord files aren't added
    // public bool RawFilesHaveError()
    // {
    //     foreach (IDatafile file in RawFiles.Values)
    //     {
    //         if (file.IsReadyToProcess)
    //         {
    //             return false;
    //         }
    //     }
    //     return true;
    // }
    // TODO can remove once errord files aren't added

    // public Dictionary<string, List<string>> GetDataFileErrors()
    // {
    //     Dictionary<string, List<string>> fileErrors = [];
    //     foreach (KeyValuePair<DateTime, IDatafile> entry in RawFiles)
    //     {

    //         IDatafile value = entry.Value;
    //         string fileName = value.GetDataFileName();

    //         fileErrors.Add(fileName, value.Errors);
    //         foreach (string err in value.Errors)
    //         {

    //             Console.WriteLine($"filename errors in booking: {err}");
    //         }
    //     }
    //     return fileErrors;
    // }
    public void AddDataFile(IDatafile rawDataFile)
    {
        DateTime dfStartTime = rawDataFile.StartTime;
        RawFiles.Add(dfStartTime, rawDataFile);
    }

    public bool IsWithinBookingWindow(IDatafile otherFile)
    {

        if (RawFiles.Count == 0)
        {
            return true;
        }

        IDatafile firstFile = RawFiles.Values.FirstOrDefault()!;

        IDatafile lastFile = RawFiles.Values.LastOrDefault()!;


        if (Math.Abs((firstFile.StartTime - otherFile.EndTime).TotalSeconds) >= 3600 &&
             Math.Abs((otherFile.StartTime - lastFile.EndTime).TotalSeconds) >= 3600)
        {
            return false;
        }

        return true;
    }
    public List<string> GetDataFileNames()
    {
        List<string> dataFileNames = [];
        foreach (KeyValuePair<DateTime, IDatafile> df in RawFiles)
        {
            dataFileNames.Add(df.Value.GetDataFileName());
        }
        return dataFileNames;
    }

}