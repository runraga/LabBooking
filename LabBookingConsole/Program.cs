using LabBookingLib.Models;
using LabBookingLib.Services;



string folderPath = args.Length == 0 ? "../datafiles/24001_test-files" : args[0];

string[] filesInProject = Directory.GetFiles(folderPath);

// TODO: if no files then exit.

List<IBooking> bookingsForProject = [InstrumentBookerFactory.GetNewBooker(folderPath)];

foreach (string filesName in filesInProject)
{
    //TODO need factory for IDataFile implementation
    IDatafile rawDataFile = new ThermoDatafile(filesName);

    bool fileAddedToBooking = false;
    // if file has an error it isn't added to a booking
    if (rawDataFile.IsReadyToProcess)
    {
        foreach (IBooking booking in bookingsForProject)
        {

            if (booking.IsWithinBookingWindow(rawDataFile))
            {
                fileAddedToBooking = true;
                booking.AddDataFile(rawDataFile);
            }
        }
        if (!fileAddedToBooking)
        {
            bookingsForProject.Add(InstrumentBookerFactory.GetNewBooker(folderPath));
            bookingsForProject.Last().AddDataFile(rawDataFile);
        }

    }
    else
    {
        Console.WriteLine($"File error:\n\t{rawDataFile.GetDataFileName()} not added to a booking.");
        foreach (string err in rawDataFile.Errors)
        {
            Console.WriteLine($"\t\tError: {err}");

        }


    }

}
// make valid bookings
foreach (IBooking booking in bookingsForProject)
{
    if (booking.IsValidBooking)
    {
        bool task = await booking.MakeBookingAsync();
        if (!task)
        {
            Console.WriteLine($"there was an error making the booking for project {booking.ProjectCode}:");
            Console.WriteLine($"\t{booking.BookingError}");
        }
    }
    else
    {
        Console.WriteLine($"An invalid booking was found for project {booking.ProjectCode}, instrument time not recorded for:");
        foreach (string df in booking.GetDataFileNames())
        {
            Console.WriteLine($"\t{df}");
        }
    }
}
