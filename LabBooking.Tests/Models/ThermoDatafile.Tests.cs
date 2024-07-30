namespace LabBooking.Tests.Models;
using LabBookingLib.Models;
using LabBooking.Tests.Services;
public class ThermoDatafileTests
{

    [Fact]
    public void ThermoDatafile_InitializesCorrectly()
    {
        // Arrange
        var filePath = "testFilePath.raw";
        var mockRawDataFile = MockRawDataFiles.SetupMockRawDataFile(false, false, DateTime.Now.AddMinutes(-100), DateTime.Now, 60, filePath);


        // Act
        var thermoDatafile = new ThermoDatafile(mockRawDataFile.Object);

        // Assert
        Assert.True(thermoDatafile.IsReadyToProcess);
        Assert.Empty(thermoDatafile.Errors);
        Assert.Equal(mockRawDataFile.Object.FileHeader.CreationDate, thermoDatafile.StartTime);
        Assert.Equal(60, thermoDatafile.DurationMinutes);
        Assert.Equal(thermoDatafile.StartTime.AddMinutes(60), thermoDatafile.EndTime);
        Assert.Equal(filePath, thermoDatafile.GetDataFileName());
    }

    [Fact]
    public void CheckReadyToProcess_FileInAcquisitionOlderThan48Hours_SetsErrorsAndIsNotReady()
    {
        // Arrange
        var filePath = "testFilePath.raw";
        var creationDate = DateTime.Now.AddDays(-7);
        var modifiedDate = DateTime.Now.AddDays(-6);
        var mockRawDataFile = MockRawDataFiles.SetupMockRawDataFile(true, false, creationDate, modifiedDate, 60, filePath);

        // Act
        var thermoDatafile = new ThermoDatafile(mockRawDataFile.Object);


        // Assert
        Assert.False(thermoDatafile.IsReadyToProcess);
        Assert.Contains("This data file is older than 48 hours but showing as in acquisition.", thermoDatafile.Errors);
    }

    [Fact]
    public void CheckReadyToProcess_FileInAcquisitionWithin48Hours_SetsErrorsAndIsNotReady()
    {
        // Arrange
        var filePath = "testFilePath.raw";
        var creationDate = DateTime.Now;
        var modifiedDate = DateTime.Now;
        var mockRawDataFile = MockRawDataFiles.SetupMockRawDataFile(true, false, creationDate, modifiedDate, 60, filePath);

        // Act
        var thermoDatafile = new ThermoDatafile(mockRawDataFile.Object);

        // Assert
        Assert.False(thermoDatafile.IsReadyToProcess);
        Assert.Contains("This datafile is in acquisition and won't backed up until finished.", thermoDatafile.Errors);
    }

    [Fact]
    public void CheckReadyToProcess_FileHasError_SetsErrorsAndIsNotReady()
    {
        // Arrange
        var filePath = "testFilePath.raw";
        var creationDate = DateTime.Now;
        var modifiedDate = DateTime.Now;
        var mockRawDataFile = MockRawDataFiles.SetupMockRawDataFile(false, true, creationDate, modifiedDate, 60, filePath, "File read error");

        // Act
        var thermoDatafile = new ThermoDatafile(mockRawDataFile.Object);

        // Assert
        Assert.False(thermoDatafile.IsReadyToProcess);
        Assert.Contains("Data file error: File read error.", thermoDatafile.Errors);
    }

    [Fact]
    public void CheckReadyToProcess_ValidFile_IsReadyToProcess()
    {
        // Arrange
        var filePath = "testFilePath.raw";
        var creationDate = DateTime.Now;
        var modifiedDate = DateTime.Now;
        var mockRawDataFile = MockRawDataFiles.SetupMockRawDataFile(false, false, creationDate, modifiedDate, 60, filePath);

        // Act
        var thermoDatafile = new ThermoDatafile(mockRawDataFile.Object);

        // Assert
        Assert.True(thermoDatafile.IsReadyToProcess);
        Assert.Empty(thermoDatafile.Errors);
    }
}