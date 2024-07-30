namespace LabBooking.Tests.Resources;
using LabBooking.Tests.Services;
using ThermoFisher.CommonCore.Data.Interfaces;

using Moq;
public static class BookingModelTestData
{
    public static IEnumerable<object[]> HappyPathDataWithZero()
    {
        yield return new object[] { BookingModelTestLibrary.GetZeroFileHappy(), new List<string>() { } };

        foreach (var data in HappyPathDataWithoutZero())
        {
            yield return data;
        }
    }
    public static IEnumerable<object[]> HappyPathDataWithoutZero()
    {
        yield return new object[] { BookingModelTestLibrary.GetOneFileHappy(), new List<string>() { "test-file-path.raw01" } };

        yield return new object[] { BookingModelTestLibrary.GetTwoFilesHappy(), new List<string>() { "test-file-path.raw01", "test-file-path02.raw" } };

        yield return new object[] { BookingModelTestLibrary.GetThreeFilesHappy(), new List<string>() { "test-file-path.raw01", "test-file-path02.raw", "test-file-path03.raw" } };
    }
    public static IEnumerable<object[]> BookingStartTimeHappyPathDataWithoutZero()
    {
        DateTime dt = new(2023, 1, 1, 12, 0, 0);

        yield return new object[] { BookingModelTestLibrary.GetOneFileHappy(), new List<DateTime>() { dt } };

        yield return new object[] { BookingModelTestLibrary.GetThreeFilesHappy(), new List<DateTime>() { dt, dt.AddMinutes(65), dt.AddMinutes(130) } };



    }
    public static IEnumerable<object[]> BookingDurationHappyPathDataWithZero()
    {
        yield return new object[] { BookingModelTestLibrary.GetZeroFileHappy(), 0 };

        yield return new object[] { BookingModelTestLibrary.GetOneFileHappy(), 60 };

        yield return new object[] { BookingModelTestLibrary.GetTwoFilesHappy(), 125 };

        yield return new object[] { BookingModelTestLibrary.GetThreeFilesHappy(), 190 };

        yield return new object[] { BookingModelTestLibrary.GetPartialMinutesExample(), 250 };


    }

    public static IEnumerable<object[]> OutsideBookingWindowData()
    {
        DateTime dt = new(2023, 1, 1, 12, 0, 0);
        yield return new object[] {
            new List<Mock<IRawDataExtended>>{
                MockRawDataFiles.SetupMockRawDataFile(false, false, dt, dt, 60, "test-file-path.raw01"),
                MockRawDataFiles.SetupMockRawDataFile(false, false, dt.AddHours(5), dt.AddHours(5), 60, "test-file-path.raw02")
            },

            new List<string>() { "test-file-path.raw01" } };

        yield return new object[] {
            new List<Mock<IRawDataExtended>>{
                MockRawDataFiles.SetupMockRawDataFile(false, false, dt, dt, 60, "test-file-path.raw01"),
                MockRawDataFiles.SetupMockRawDataFile(false, false, dt.AddMinutes(120), dt.AddMinutes(120), 60, "test-file-path.raw02")
            },

            new List<string>() { "test-file-path.raw01" } };
    }
    public static IEnumerable<object[]> ShortBookingData()
    {
        yield return new object[] {
            new List<Mock<IRawDataExtended>>{
                MockRawDataFiles.SetupMockRawDataFile(false, false, DateTime.Now, DateTime.Now, 5, "test-file-path.raw01"),
            } };
        yield return new object[] {
            new List<Mock<IRawDataExtended>>{
                MockRawDataFiles.SetupMockRawDataFile(false, false, DateTime.Now, DateTime.Now, 5, "test-file-path.raw01"),
                MockRawDataFiles.SetupMockRawDataFile(false, false, DateTime.Now.AddMinutes(6), DateTime.Now.AddMinutes(6), 5, "test-file-path.raw02"),
            } };
        yield return new object[] {
            new List<Mock<IRawDataExtended>>{
                MockRawDataFiles.SetupMockRawDataFile(false, false, DateTime.Now, DateTime.Now, 15, "test-file-path.raw01"),

            } };
    }

    // public static IEnumerable<object[]> HappyPathData()
    // {
    //     yield return new object[] { BookingModelTestLibrary.GetZeroFileHappy(), new List<string>() { } };

    //     yield return new object[] { BookingModelTestLibrary.GetOneFileHappy(), new List<string>() { "test-file-path.raw01" } };

    //     yield return new object[] { BookingModelTestLibrary.GetTwoFilesHappy(), new List<string>() { "test-file-path.raw01", "test-file-path02.raw" } };

    //     yield return new object[] { BookingModelTestLibrary.GetThreeFilesHappy(), new List<string>() { "test-file-path.raw01", "test-file-path02.raw", "test-file-path03.raw" } };
}
public static class BookingModelTestLibrary
{
    private static DateTime dt = new(2023, 1, 1, 12, 0, 0);
    public static List<Mock<IRawDataExtended>> GetZeroFileHappy()
    {
        return [];
    }
    public static List<Mock<IRawDataExtended>> GetOneFileHappy()
    {
        string filePath = "test-file-path.raw01";
        return [MockRawDataFiles.SetupMockRawDataFile(false, false, dt, dt, 60, filePath)];
    }
    public static List<Mock<IRawDataExtended>> GetTwoFilesHappy()
    {
        string filePath = "test-file-path02.raw";

        return [.. GetOneFileHappy(),
        MockRawDataFiles.SetupMockRawDataFile(false, false, dt.AddMinutes(65), dt.AddMinutes(65), 60, filePath)];
    }
    public static List<Mock<IRawDataExtended>> GetThreeFilesHappy()
    {
        string filePath = "test-file-path03.raw";

        return [.. GetTwoFilesHappy(),
        MockRawDataFiles.SetupMockRawDataFile(false, false, dt.AddMinutes(130), dt.AddMinutes(130), 60, filePath)];
    }


    public static List<Mock<IRawDataExtended>> GetPartialMinutesExample()
    {
        DateTime dt = new(2023, 1, 1, 12, 0, 0);
        return [..BookingModelTestLibrary.GetThreeFilesHappy(),
                    MockRawDataFiles.SetupMockRawDataFile(false, false, dt.AddMinutes(190), dt.AddMinutes(190), 59.4, "test-file-path.raw01")];
    }
}

