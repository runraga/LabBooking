namespace LabBooking.Tests.BookingSystem;

public static class RatesEnums
{

    public static IEnumerable<object[]> DetermineRateIdTestData()
    {
        // valid dict and list
        yield return new object[] {new Dictionary<string, int>{
                                                                { "669f6d6c6ee12e497ebcc794", 50 }, //internal
                                                                { "669f6d6c6ee12e497ebcc795", 100 }, //external
                                                              },
                                   new List<string>{
                                                        "669f6d6c6ee12e497ebcc794", "669f6d6c6ee12e497ebcc795"
                                                    },
                                                    "669f6d6c6ee12e497ebcc795"};
        //different list
        yield return new object[] {new Dictionary<string, int>{
                                                                { "669f6d6c6ee12e497ebcc794", 50 }, //internal
                                                                { "669f6d6c6ee12e497ebcc795", 100 }, //external
                                                              },
                                   new List<string>{
                                                        "669f6d6c6ee12e497ebcc794", "1234", "2345"
                                                    },
                                                    "669f6d6c6ee12e497ebcc794"};
        // no matching elements in the dict vs the list - need an exception check for that

        // rates in dict or list not in the hierarchy - this would give an exception too and needs to be checked separatly.

    }
    public static IEnumerable<object[]> DeterminRateIdExceptionTestData()
    {

        // no matching elements in the dict vs the list - need an exception check for that
        yield return new object[] {new Dictionary<string, int>{
                                                                { "669f6d6c6ee12e497ebcc794", 50 }, //internal
                                                                { "669f6d6c6ee12e497ebcc795", 100 }, //external
                                                              },
                                   new List<string>{
                                                        "669f6d6c6ee12e497ebcc79a", "669f6d6c6ee12e497ebcc79b"
                                                    },
                                                    new Exception("Charge rate could not be determined, check user and resource usergroups")};
        // rates in dict or list not in the hierarchy - this would give an exception too and needs to be checked separatly.
        yield return new object[] {new Dictionary<string, int>{
                                                                { "669f6d6c6ee12e497ebcc79a", 50 }, //internal
                                                                { "669f6d6c6ee12e497ebcc79b", 100 }, //external
                                                              },
                                   new List<string>{
                                                        "669f6d6c6ee12e497ebcc79a", "669f6d6c6ee12e497ebcc79b"
                                                    },
                                                    new Exception("Charge rate could not be determined, check user and resource usergroups")};
    }
}