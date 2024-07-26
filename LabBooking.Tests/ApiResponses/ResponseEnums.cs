namespace LabBooking.Tests.ApiResponses;

using System.Security.Cryptography.X509Certificates;
using System.Text.Json;



public static class ResponseHelpers
{

    public static IEnumerable<object[]> GetProjectIdExceptionTestData()
    {
        // Data object not an array
        yield return new object[] { @$"{{""Data"":{{""_id"": true}},""Success"":{JsonSerializer.Serialize(true)}}}" };
        // First element does not contain _id key
        yield return new object[] { @$"{{""Data"":[{{""acknowledged"": false}}],""Success"":{JsonSerializer.Serialize(true)}}}" };
        // multiple projects returned
        yield return new object[] { @$"{{""Data"":[{{""acknowledged"": false}},{{""acknowledged"": false}}],""Success"":{JsonSerializer.Serialize(true)}}}" };
        // no project objects returned
        yield return new object[] { @$"{{""Data"":[],""Success"":{JsonSerializer.Serialize(true)}}}" };

    }
    public static IEnumerable<object[]> GetRequestInfoExceptionTestData()
    {
        // No requests on object - empty array
        yield return new object[] { @$"{{
                                ""Data"": 
                                [
                                    {{
                                        ""_id"": ""669f6d6d6ee12e497ebcc7a0"",
                                        ""name"": ""24001_an_eclipse_project"",
                                        ""requests"": 
                                            [
                                            ],
                                        ""bookings"": [ ]
                                    }}
                                ],""Success"":{JsonSerializer.Serialize(true)}
                                }}", new KeyNotFoundException("No requests found on that project") };
        // no requests key on object
        yield return new object[] { @$"{{
                    ""Data"": 
                    [
                        {{
                            ""_id"": ""669f6d6d6ee12e497ebcc7a0"",
                            ""name"": ""24001_an_eclipse_project"",
                            ""bookings"": [ ]
                        }}
                    ],""Success"":{JsonSerializer.Serialize(true)}
                    }}", new KeyNotFoundException("No requests found on that project") };
        // request with valid request name not found
        yield return new object[] { @$"{{
                    ""Data"": 
                    [
                        {{
                            ""_id"": ""669f6d6d6ee12e497ebcc7a0"",
                            ""name"": ""24001_an_eclipse_project"",
                            ""requests"": 
                                [
                                    {{
                                        ""_id"": ""669f6d6d6ee12e497ebcc79e"",
                                        ""users"": ""669f6d6c6ee12e497ebcc796"",
                                        ""name"": ""An invalid Request"",
                                        ""account_number"": ""19300001""
                                    }}
                                ],
                            ""bookings"": [ ]
                        }}
                    ],""Success"":{JsonSerializer.Serialize(true)}
                    }}", new InvalidOperationException("A valid request could not be found") };

        yield return new object[] { @$"{{
                    ""Data"": 
                    [
                        {{
                            ""_id"": ""669f6d6d6ee12e497ebcc7a0"",
                            ""name"": ""24001_an_eclipse_project"",
                            ""requests"": 
                                [
                                    {{
                                        ""_id"": ""669f6d6d6ee12e497ebcc79e"",
                                        ""users"": ""669f6d6c6ee12e497ebcc796"",
                                        ""name"": ""Proteomics Request""
                                    }}
                                ],
                            ""bookings"": [ ]
                        }}
                    ],""Success"":{JsonSerializer.Serialize(true)}
                    }}" ,
                         new KeyNotFoundException("a valid user or account number could not be found")

                    };
    }
    public static bool DictionariesAreEqual<TKey, TValue>(IDictionary<TKey, TValue> dict1, IDictionary<TKey, TValue> dict2)
    {
        if (dict1.Count != dict2.Count)
        {
            Console.WriteLine("dictionaries are not the same size");
            return false;
        }

        foreach (var kvp in dict1)
        {
            if (!dict2.TryGetValue(kvp.Key, out var value) || !Equals(kvp.Value, value))
            {

                return false;
            }
        }

        return true;
    }

}


