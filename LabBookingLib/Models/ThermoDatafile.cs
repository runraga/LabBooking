namespace LabBookingLib.Models;
using ThermoFisher.CommonCore.RawFileReader;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;

public class ThermoDatafile : IDatafile
{
    public readonly IRawDataExtended RawDataFile;
    public bool IsReadyToProcess { get; } = false;
    public List<string> Errors { get; } = [];
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public int DurationMinutes { get; }


    public ThermoDatafile(IRawDataExtended rdf)
    {
        RawDataFile = rdf;
        RawDataFile.SelectInstrument(Device.MS, 1);
        IsReadyToProcess = CheckReadyToProcess();
        Console.WriteLine(IsReadyToProcess);
        StartTime = RawDataFile.FileHeader.CreationDate;
        DurationMinutes = (int)Math.Ceiling(RawDataFile.RunHeaderEx.EndTime);
        EndTime = StartTime.AddMinutes(DurationMinutes);
    }

    public ThermoDatafile(string filePath) : this(RawFileReaderAdapter.FileFactory(filePath)) { }

    private bool CheckReadyToProcess()
    {
        bool readyCheck = true;
        Console.WriteLine($"ready check at start of method:{readyCheck}");
        Console.WriteLine($"Is it in acquisition:{RawDataFile.InAcquisition}");

        if (RawDataFile.InAcquisition)
        {
            DateTime acquiredTime = RawDataFile.FileHeader.ModifiedDate;
            bool is_old_file = acquiredTime < DateTime.Now.AddHours(-48);
            if (is_old_file)
            {
                Errors.Add("This data file is older than 48 hours but showing as in acquisition.");

                readyCheck = false;
            }
            else
            {
                Errors.Add("This datafile is in acquisition and won't backed up until finished.");
                readyCheck = false;
            }
        }
        Console.WriteLine($"Is it in error:{RawDataFile.IsError}");

        if (RawDataFile.IsError)
        {
            Console.WriteLine("errors found");
            Errors.Add($"Data file error: {RawDataFile.FileError.ErrorMessage}.");


            readyCheck = false;
        }
        Console.WriteLine($"ready check at end of method:{readyCheck}");


        return readyCheck;
    }
    public string GetDataFileName()
    {
        return RawDataFile.FileName;
    }
}

