namespace LabBooking.Tests.Models;
using LabBookingLib.Models;
using ThermoFisher.CommonCore.Data.Interfaces;
using Moq;
public class ThermoDatafileTests
{
    private Mock<IRawDataExtended> SetupMockRawDataFile(bool inAcquisition, bool isError, DateTime creationDate, DateTime modifiedDate, double endTime, string fileName, string fileError = null)
    {
        var mockRawDataFile = new Mock<IRawDataExtended>();

        mockRawDataFile.Setup(r => r.InAcquisition).Returns(inAcquisition);
        mockRawDataFile.Setup(r => r.IsError).Returns(isError);
        mockRawDataFile.Setup(r => r.FileHeader.CreationDate).Returns(creationDate);
        mockRawDataFile.Setup(r => r.FileHeader.ModifiedDate).Returns(modifiedDate);
        mockRawDataFile.Setup(r => r.RunHeaderEx.EndTime).Returns(endTime);
        mockRawDataFile.Setup(r => r.FileName).Returns(fileName);
        if (isError)
        {
            Mock<IFileError> mockFileError = new();
            mockFileError.Setup(r => r.ErrorMessage).Returns("There is a file error");
            mockRawDataFile.Setup(r => r.FileError).Returns(mockFileError.Object);
        }

        return mockRawDataFile;
    }

    [Fact]
    public void ThermoDatafile_InitializesCorrectly()
    {
        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        // Arrange
        var filePath = "testFilePath.raw";
        var mockRawDataFile = SetupMockRawDataFile(false, false, DateTime.Now.AddMinutes(-100), DateTime.Now, 60, filePath);


        // Act
        var thermoDatafile = new ThermoDatafile(mockRawDataFile.Object);

        string output = stringWriter.ToString();
        string fp = "..\\..\\..\\output\\property_finder_output.txt";
        File.WriteAllText(fp, output);
        // Assert
        Assert.True(thermoDatafile.IsReadyToProcess);
        Assert.Empty(thermoDatafile.Errors);
        Assert.Equal(mockRawDataFile.Object.FileHeader.CreationDate, thermoDatafile.StartTime);
        Assert.Equal(60, thermoDatafile.DurationMinutes);
        Assert.Equal(thermoDatafile.StartTime.AddMinutes(60), thermoDatafile.EndTime);
        Assert.Equal(filePath, thermoDatafile.GetDataFileName());
    }

    // [Fact]
    // public void CheckReadyToProcess_FileInAcquisitionOlderThan48Hours_SetsErrorsAndIsNotReady()
    // {
    //     // Arrange
    //     var filePath = "testFilePath.raw";
    //     var creationDate = DateTime.Now.AddDays(-3);
    //     var modifiedDate = DateTime.Now.AddDays(-3);
    //     var mockRawDataFile = SetupMockRawDataFile(true, false, creationDate, modifiedDate, 60, "testFile.raw");
    //     RawFileReaderAdapter.FileFactory = (path) => mockRawDataFile.Object;

    //     // Act
    //     var thermoDatafile = new ThermoDatafile(filePath);

    //     // Assert
    //     Assert.False(thermoDatafile.IsReadyToProcess);
    //     Assert.Contains("This data file is older than 48 hours but showing as in acquisition.", thermoDatafile.Errors);
    // }

    // [Fact]
    // public void CheckReadyToProcess_FileInAcquisitionWithin48Hours_SetsErrorsAndIsNotReady()
    // {
    //     // Arrange
    //     var filePath = "testFilePath.raw";
    //     var creationDate = DateTime.Now;
    //     var modifiedDate = DateTime.Now;
    //     var mockRawDataFile = SetupMockRawDataFile(true, false, creationDate, modifiedDate, 60, "testFile.raw");
    //     RawFileReaderAdapter.FileFactory = (path) => mockRawDataFile.Object;

    //     // Act
    //     var thermoDatafile = new ThermoDatafile(filePath);

    //     // Assert
    //     Assert.False(thermoDatafile.IsReadyToProcess);
    //     Assert.Contains("This datafile is in acquisition and won't backed up until finished.", thermoDatafile.Errors);
    // }

    // [Fact]
    // public void CheckReadyToProcess_FileHasError_SetsErrorsAndIsNotReady()
    // {
    //     // Arrange
    //     var filePath = "testFilePath.raw";
    //     var creationDate = DateTime.Now;
    //     var modifiedDate = DateTime.Now;
    //     var mockRawDataFile = SetupMockRawDataFile(false, true, creationDate, modifiedDate, 60, "testFile.raw", "File read error");
    //     RawFileReaderAdapter.FileFactory = (path) => mockRawDataFile.Object;

    //     // Act
    //     var thermoDatafile = new ThermoDatafile(filePath);

    //     // Assert
    //     Assert.False(thermoDatafile.IsReadyToProcess);
    //     Assert.Contains("Data file error: File read error.", thermoDatafile.Errors);
    // }

    // [Fact]
    // public void CheckReadyToProcess_ValidFile_IsReadyToProcess()
    // {
    //     // Arrange
    //     var filePath = "testFilePath.raw";
    //     var creationDate = DateTime.Now;
    //     var modifiedDate = DateTime.Now;
    //     var mockRawDataFile = SetupMockRawDataFile(false, false, creationDate, modifiedDate, 60, "testFile.raw");
    //     RawFileReaderAdapter.FileFactory = (path) => mockRawDataFile.Object;

    //     // Act
    //     var thermoDatafile = new ThermoDatafile(filePath);

    //     // Assert
    //     Assert.True(thermoDatafile.IsReadyToProcess);
    //     Assert.Empty(thermoDatafile.Errors);
    // }
}