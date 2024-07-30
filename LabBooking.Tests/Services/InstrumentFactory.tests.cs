namespace LabBooking.Tests.Services;
using LabBookingLib.Models;
using LabBookingLib.Services;


public class InstrumentBookerFactory_Should
{
    [Fact]
    public void GetNewBooker_ReturnsIBookingObjectWithEclipseIInstrument()
    {
        string path = "c:\\QC_test_folder";

        IBooking result = InstrumentBookerFactory.GetNewBooker(path);

        Assert.True(result.Instrument is Eclipse);
    }
    [Fact]
    public void GetNewBooker_ReturnsProjectBookingIfFolderStartsWithQC()
    {
        string path = "c:\\QC_test_folder";

        IBooking result = InstrumentBookerFactory.GetNewBooker(path);

        Assert.True(result is ProjectBooking);
    }
    [Fact]
    public void GetNewBooker_ReturnsProjectBookingIfFolderStartsWithOPT()
    {
        string path = "c:\\OPT_test_folder";

        IBooking result = InstrumentBookerFactory.GetNewBooker(path);

        Assert.True(result is ProjectBooking);
    }
    [Fact]
    public void GetNewBooker_ReturnsProjectBookingIfFolderStartsWithFiveDigits()
    {
        string path = "c:\\24001_test_folder";

        IBooking result = InstrumentBookerFactory.GetNewBooker(path);

        Assert.True(result is ProjectBooking);
    }
}


