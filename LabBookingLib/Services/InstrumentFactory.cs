namespace LabBookingLib.Services;
using System.Text.RegularExpressions;
using LabBookingLib.Models;
using LabBookingLib.BookingSystem;


public static class InstrumentBookerFactory
{
    // public static IBookingSystem GetNewBooker(string folder)
    // {
    //     // Determine booking API object to use

    //     IInstrument instrumentAPI;

    //     if (folder.Contains("c\\:"))
    //     {
    //         instrumentAPI = new Eclipse();
    //     }
    //     else
    //     {
    //         // TODO: make default implementation? Or error?
    //         instrumentAPI = new Eclipse();
    //     }

    //     // Determine booking object to use base on project type

    //     DirectoryInfo di = new(folder);

    //     string projectCode = di.Name.Split('_')[0];

    //     if (projectCode == "QC")
    //     {
    //         return new ProjectBooking(projectCode, instrumentAPI);
    //     }
    //     else if (projectCode == "OPT")
    //     {
    //         return new ProjectBooking(projectCode, instrumentAPI);
    //     }
    //     else if (Regex.IsMatch(projectCode, @"^\d{5}$"))
    //     {
    //         return new ProjectBooking(projectCode, instrumentAPI);
    //     }
    //     else
    //     {
    //         throw new ArgumentException("Invalid project code");
    //     }
    // }

}
