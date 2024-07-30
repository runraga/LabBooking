namespace LabBooking.Tests.Services;
using Moq;
using ThermoFisher.CommonCore.Data.Interfaces;

public static class MockRawDataFiles
{
    public static Mock<IRawDataExtended> SetupMockRawDataFile(bool inAcquisition, bool isError, DateTime creationDate, DateTime modifiedDate, double endTime, string fileName, string fileError = null)
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
            // Mock<IFileError> mockFileError = new();
            // mockFileError.Setup(r => r.ErrorMessage).Returns("There is a file error");
            mockRawDataFile.Setup(r => r.FileError.ErrorMessage).Returns(fileError);
        }

        return mockRawDataFile;
    }
}