
namespace LabBooking.Tests.Models;
using LabBooking.Tests.Resources;
using LabBooking.Tests.Services;
using LabBookingLib.Models;
using Moq;
using ThermoFisher.CommonCore.Data.Interfaces;

public class ProjectBooking_Should
{

    [Theory]
    [MemberData(nameof(BookingModelTestData.HappyPathDataWithZero), MemberType = typeof(BookingModelTestData))]
    public void GetDataFileNames_ReturnsCorrectList(List<Mock<IRawDataExtended>> dataFiles, List<string> expected)
    {
        //Arrange
        ProjectBooking pb = new("test-code", new Eclipse());
        //Act
        foreach (Mock<IRawDataExtended> df in dataFiles)
        {
            IDatafile rdf = new ThermoDatafile(df.Object);
            pb.AddDataFile(rdf);
        }
        //Assert
        Assert.Equal(expected, pb.GetDataFileNames());
    }
    [Theory]
    [MemberData(nameof(BookingModelTestData.HappyPathDataWithZero), MemberType = typeof(BookingModelTestData))]
    public void IsWithinBookingWindow_ReturnsTrueIfTimeGapsAreLessThan60Mins(List<Mock<IRawDataExtended>> dataFiles, List<string> expected)
    {
        //Arrange
        ProjectBooking pb = new("test-code", new Eclipse());
        //Act

        //Assert
        foreach (Mock<IRawDataExtended> df in dataFiles)
        {
            IDatafile nextFile = new ThermoDatafile(df.Object);
            bool isWithinWindow = pb.IsWithinBookingWindow(nextFile);
            if (isWithinWindow)
            {
                pb.AddDataFile(nextFile);
            }
            Assert.True(isWithinWindow);
        }

    }
    [Theory]
    [MemberData(nameof(BookingModelTestData.OutsideBookingWindowData), MemberType = typeof(BookingModelTestData))]
    public void IsWithinBookingWindow_ReturnsFalseIfTimeGapsAreMoreThan60Mins(List<Mock<IRawDataExtended>> dataFiles, List<string> expected)
    {
        //Arrange
        ProjectBooking pb = new("test-code", new Eclipse());
        //Act

        IDatafile rdf = new ThermoDatafile(dataFiles[0].Object);
        pb.AddDataFile(rdf);

        //Assert
        for (int i = 1; i < dataFiles.Count; i++)
        {
            PrintOutput.Start();
            IDatafile nextFile = new ThermoDatafile(dataFiles[i].Object);
            bool isWithinWindow = pb.IsWithinBookingWindow(nextFile);
            if (isWithinWindow)
            {
                pb.AddDataFile(nextFile);
            }
            PrintOutput.Stop();
            Assert.False(isWithinWindow);

        }

    }

    [Theory]
    [MemberData(nameof(BookingModelTestData.HappyPathDataWithZero), MemberType = typeof(BookingModelTestData))]
    public void AddDataFile_HasCorrectNumberOfDatafilesAdded
    (List<Mock<IRawDataExtended>> dataFiles, List<string> expected)
    {
        //Arrange
        ProjectBooking pb = new("test-code", new Eclipse());
        //Act
        foreach (Mock<IRawDataExtended> df in dataFiles)
        {
            IDatafile rdf = new ThermoDatafile(df.Object);
            pb.AddDataFile(rdf);
        }
        //Assert
        Assert.Equal(expected.Count, pb.GetDataFileNames().Count);
    }
    [Theory]
    [MemberData(nameof(BookingModelTestData.BookingStartTimeHappyPathDataWithoutZero), MemberType = typeof(BookingModelTestData))]
    public void GetStartTime_ReturnsCorrectStartTime(List<Mock<IRawDataExtended>> dataFiles, List<DateTime> expected)
    {
        //add files in reverse order
        //Arrange
        ProjectBooking pb = new("test-code", new Eclipse());
        //Act
        for (int i = dataFiles.Count - 1; i > -1; i--)
        {
            {
                IDatafile rdf = new ThermoDatafile(dataFiles[i].Object);
                pb.AddDataFile(rdf);
            }
            //Assert
            Assert.Equal(expected[i], pb.GetBookingStartTime());
        }
    }
    [Fact]
    public void GetBookingStartTime_ThrowsExceptionWhenNoDataFiles()
    {
        ProjectBooking pb = new("test-code", new Eclipse());

        Exception err = Assert.Throws<InvalidOperationException>(() => pb.GetBookingStartTime());

        Assert.Equal("Not data files in booking, no start time available", err.Message);
    }

    [Theory]
    [MemberData(nameof(BookingModelTestData.BookingDurationHappyPathDataWithZero), MemberType = typeof(BookingModelTestData))]
    public void BookingLengthMinutes_ReturnsCorrectLengthInMinutes(List<Mock<IRawDataExtended>> dataFiles, int expected)
    {
        //add files in reverse order
        //Arrange
        ProjectBooking pb = new("test-code", new Eclipse());
        //Act
        foreach (Mock<IRawDataExtended> df in dataFiles)
        {
            {
                IDatafile rdf = new ThermoDatafile(df.Object);
                pb.AddDataFile(rdf);
            }
        }
        //Assert
        Assert.Equal(expected, pb.BookingLengthMinutes);
    }
    //      GetBookingStartTime
    //          TODO: what if files being added are out of sequence
    //                  could be outside booking window initially but not later...?
    //                  does it matter as it would be added as its own booking?
    //                  could we sort datafiles before adding? ie make IDatafile and then sort
    [Theory]
    [MemberData(nameof(BookingModelTestData.HappyPathDataWithoutZero), MemberType = typeof(BookingModelTestData))]
    public void IsValidBooking_ReturnsTrueForBookingsLongerThan15Mins(List<Mock<IRawDataExtended>> dataFiles, List<string> expected)
    {
        //Arrange
        ProjectBooking pb = new("test-code", new Eclipse());

        //Act
        foreach (Mock<IRawDataExtended> df in dataFiles)
        {
            IDatafile rdf = new ThermoDatafile(df.Object);
            pb.AddDataFile(rdf);
        }
        //Assert
        Assert.True(pb.IsValidBooking);
    }
    [Fact]
    public void IsValidBooking_ReturnsFalseWhenNoDataFiles()
    {
        ProjectBooking pb = new("test-code", new Eclipse());

        Assert.False(pb.IsValidBooking);
    }
    [Theory]
    [MemberData(nameof(BookingModelTestData.ShortBookingData), MemberType = typeof(BookingModelTestData))]
    public void IsValidBooking_ReturnsFalseForShortBookings(List<Mock<IRawDataExtended>> dataFiles)
    {
        //Arrange
        ProjectBooking pb = new("test-code", new Eclipse());

        //Act
        foreach (Mock<IRawDataExtended> df in dataFiles)
        {
            IDatafile rdf = new ThermoDatafile(df.Object);
            pb.AddDataFile(rdf);
        }
        //Assert
        Assert.False(pb.IsValidBooking);
    }

    [Fact]
    public async Task MakeBookingAsync_ReturnsTrueWithSuccessfulBooking()
    {
        var mockInstrument = new Mock<IInstrument>();
        mockInstrument.Setup(api => api.MakeBookingAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<double>()))
                                                        .ReturnsAsync(true);
        ProjectBooking pb = new("test-code", mockInstrument.Object);
        List<Mock<IRawDataExtended>> dfs = BookingModelTestLibrary.GetThreeFilesHappy();
        foreach (Mock<IRawDataExtended> df in dfs)
        {
            IDatafile rdf = new ThermoDatafile(df.Object);
            pb.AddDataFile(rdf);
        }
        bool result = await pb.MakeBookingAsync();
        Assert.True(result);
        Assert.Equal("No booking errors found", pb.BookingError);
        Assert.True(pb.BookingComplete);

    }
    [Fact]
    public async Task MakeBookingAsync_CallsInstrumentWithCorrectParameters()
    {
        var mockInstrument = new Mock<IInstrument>();
        mockInstrument.Setup(api => api.MakeBookingAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<double>()))
                                                        .ReturnsAsync(true);
        ProjectBooking pb = new("test-code", mockInstrument.Object);
        List<Mock<IRawDataExtended>> dfs = BookingModelTestLibrary.GetThreeFilesHappy();
        foreach (Mock<IRawDataExtended> df in dfs)
        {
            IDatafile rdf = new ThermoDatafile(df.Object);
            pb.AddDataFile(rdf);
        }
        await pb.MakeBookingAsync();
        // Assert
        mockInstrument.Verify(api => api.MakeBookingAsync(
            "test-code",
            new DateTime(2023, 1, 1, 12, 0, 0),
            190
            ),
            Times.Once);


    }
    [Fact]
    public async Task MakeBookingAsyn_CatchesExceptionsProperly()
    {
        var mockInstrument = new Mock<IInstrument>();
        mockInstrument.Setup(api => api.MakeBookingAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<double>()))
                        .ThrowsAsync(new("Could not get project ID for current project."));

        ProjectBooking pb = new("test-code", mockInstrument.Object);
        List<Mock<IRawDataExtended>> dfs = BookingModelTestLibrary.GetThreeFilesHappy();
        foreach (Mock<IRawDataExtended> df in dfs)
        {
            IDatafile rdf = new ThermoDatafile(df.Object);
            pb.AddDataFile(rdf);
        }
        bool result = await pb.MakeBookingAsync();
        // Assert
        Assert.False(result);
        Assert.Equal("Could not get project ID for current project.", pb.BookingError);
    }

}