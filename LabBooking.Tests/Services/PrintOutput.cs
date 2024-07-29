namespace LabBooking.Tests.Services;

public static class PrintOutput
{
    static readonly StringWriter sw = new();
    public static void Start()
    {

        Console.SetOut(sw);
    }
    public static void Stop()
    {
        string output = sw.ToString();
        string fp = "..\\..\\..\\output\\property_finder_output.txt";
        File.WriteAllText(fp, output);
    }
}