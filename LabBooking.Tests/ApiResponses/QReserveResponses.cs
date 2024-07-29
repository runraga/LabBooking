namespace LabBooking.Tests.ApiResponses;
using LabBookingLib.ApiResponses;
using System.Text.Json;


public class QReserveResponses_PlaceBookingResponseShould
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckReturnedData_ShouldReturnValueOfAcknowledgedKey(bool value)
    {
        //arrange
        string jsonString = @$"{{""Data"":
                                    {{""acknowledged"": {JsonSerializer.Serialize(value)}
                                    }},
                                ""Success"": {JsonSerializer.Serialize(true)}
                                }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReservePlaceBookingResponse apiResponse = JsonSerializer.Deserialize<QReservePlaceBookingResponse>(response) ?? throw new Exception("Failed to deserialize the response.");

        //Act
        bool result = apiResponse.CheckReturnedData();

        Assert.Equal(value, result);


    }

    [Fact]
    public void CheckReturnedData_ShouldThrowKeyNotFoundIfKeyNotPresent()
    {

        //arrange
        string jsonString = @$"{{""Data"":
                                    {{""notCorrectKey"": {JsonSerializer.Serialize(false)}
                                    }},
                                ""Success"": {JsonSerializer.Serialize(true)}
                                }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReservePlaceBookingResponse apiResponse = JsonSerializer.Deserialize<QReservePlaceBookingResponse>(response) ?? throw new Exception("Failed to deserialize the response.");


        //Act and Assert

        Assert.Throws<KeyNotFoundException>(() => apiResponse.CheckReturnedData());


    }
}
public class QReserveResponses_GetProjectAndRequestInfoShould
{
    [Fact]
    public void GetProjectId_ReturnsValueOf_idKey()
    {
        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        //arrange
        string jsonString = @$"{{""Data"":
                                    [{{""_id"": ""a-valid-id""
                                    }}],
                                ""Success"": {JsonSerializer.Serialize(true)}
                                }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReserveGetProjectAndRequestInfo apiResponse = JsonSerializer.Deserialize<QReserveGetProjectAndRequestInfo>(response)!;

        //Act
        string result = apiResponse.GetProjectID();

        string output = stringWriter.ToString();
        string filePath = "..\\..\\..\\output\\property_finder_output.txt";
        File.WriteAllText(filePath, output);

        //Assert
        Assert.Equal("a-valid-id", result);

    }


    [Theory]
    [MemberData(nameof(ResponseHelpers.GetProjectIdExceptionTestData), MemberType = typeof(ResponseHelpers))]
    public void GetProjectId_ThrowsInvalidOperationExceptionIsReponseIncorrect(string jsonString)
    {
        //arrange
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReserveGetProjectAndRequestInfo apiResponse = JsonSerializer.Deserialize<QReserveGetProjectAndRequestInfo>(response)!;


        //Act and Assert
        Assert.Throws<InvalidOperationException>(() => apiResponse.GetProjectID());


    }

    [Fact]
    public void GetMostRecentRequestInfo_ReturnsTuple()
    {
        //arrange
        string jsonString = @$"{{
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
                                                    ""name"": ""Proteomics Request"",
                                                    ""account_number"": ""19300001""
                                                }}
                                            ],
                                        ""bookings"": [ ]
                                    }}
                                ]
                                }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReserveGetProjectAndRequestInfo apiResponse = JsonSerializer.Deserialize<QReserveGetProjectAndRequestInfo>(response)!;

        //Act
        (string, string) result = apiResponse.GetMostRecentRequestInfo();

        //Assert
        Assert.Equal(("19300001", "669f6d6c6ee12e497ebcc796"), result);
    }
    [Theory]
    [MemberData(nameof(ResponseHelpers.GetRequestInfoExceptionTestData), MemberType = typeof(ResponseHelpers))]
    public void GetProjectId_ThrowsExceptionIfResponseDataIsInvalid(string jsonString, Exception expectedException)
    {
        //arrange
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReserveGetProjectAndRequestInfo apiResponse = JsonSerializer.Deserialize<QReserveGetProjectAndRequestInfo>(response)!;


        //Act and Assert
        Exception exception = Assert.Throws(expectedException.GetType(), () => apiResponse.GetMostRecentRequestInfo());
        // Assert.Equal(expectedException.Message, exception.Message);
    }
}
public class QReserveGetRatesByResourceResponse_Should
{
    [Fact]
    public void GetRatesDict_ReturnsDictionaryStringDouble()
    {
        //arrange
        string jsonString = $@"{{
                                ""Data"": 
                                [
                                    {{
                                        ""_id"": ""669f6d6d6ee12e497ebcc79a"",
                                        ""resources"": 
                                        {{
                                            ""_id"": ""669f6d6d6ee12e497ebcc798"",
                                            ""name"": ""Microscope"",
                                            ""type"": ""Imaging""
                                        }},
                                        ""rate"": {JsonSerializer.Serialize(50)},
                                        ""currency"": ""USD"",
                                        ""user_groups"": 
                                        {{
                                            ""_id"": ""669f6d6c6ee12e497ebcc794"",
                                            ""name"": ""internal""
                                        }}
                                    }},
                                    {{
                                        ""_id"": ""669f6d6d6ee12e497ebcc79b"",
                                        ""resources"": 
                                        {{
                                            ""_id"": ""669f6d6d6ee12e497ebcc798"",
                                            ""name"": ""Microscope"",
                                            ""type"": ""Imaging""
                                        }},
                                        ""rate"": {JsonSerializer.Serialize(100)},
                                        ""currency"": ""USD"",
                                        ""user_groups"": 
                                        {{
                                            ""_id"": ""669f6d6c6ee12e497ebcc795"",
                                            ""name"": ""external""
                                        }}
                                    }}
                                ],
                                ""Success"" : {JsonSerializer.Serialize(true)}
                                }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReserveGetRatesByResourceResponse apiResponse = JsonSerializer.Deserialize<QReserveGetRatesByResourceResponse>(response)!;

        Dictionary<string, int> expected = new()
        {
            { "669f6d6c6ee12e497ebcc794", 50 }, //internal
            { "669f6d6c6ee12e497ebcc795", 100 }, //external
        };


        //Act
        Dictionary<string, int> result = apiResponse.GetRatesDict();

        //Assert
        Assert.True(ResponseHelpers.DictionariesAreEqual<string, int>(expected, result));
    }
    [Fact]
    public void GetRatesDict_ThrowsExceptionIfNoRates()
    {
        //arrange
        string jsonString = $@"{{
                                ""Data"": 
                                [
                                ]
                                }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReserveGetRatesByResourceResponse apiResponse = JsonSerializer.Deserialize<QReserveGetRatesByResourceResponse>(response)!;

        //Act & Assert
        Assert.Throws<KeyNotFoundException>(() => apiResponse.GetRatesDict());
    }
}
public class QReserveGetUserInfoResponse_Should

{
    [Fact]
    public void GetUserGroups_ReturnsListOfUserGroupIds()
    {
        string jsonString = $@"
                        {{
                            ""Data"": 
                            [
                            {{
                            ""_id"": ""669f6d6c6ee12e497ebcc796"",
                            ""name"": ""Alice"",
                            ""email"": ""alice@example.com"",
                            ""user_groups"": 
                                [
                                {{
                                    ""_id"": ""669f6d6c6ee12e497ebcc794"",
                                    ""name"": ""internal""
                                }},
                                {{
                                    ""_id"": ""669f6d6c6ee12e497ebcc795"",
                                    ""name"": ""external""
                                }}
                                ]
                            }}
                            ]
                        }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReserveGetUserInfoResponse apiResponse = JsonSerializer.Deserialize<QReserveGetUserInfoResponse>(response)!;

        List<string> expected = ["669f6d6c6ee12e497ebcc794", "669f6d6c6ee12e497ebcc795"];

        List<string> result = apiResponse.GetUserGroups();

        Assert.Equal(expected, result);
    }
    [Fact]
    public void GetUserGroups_ThrowsExceptionIfNoUserGroups()
    {
        string jsonString = $@"
                        {{
                            ""Data"": 
                            [
                            {{
                            ""_id"": ""669f6d6c6ee12e497ebcc796"",
                            ""name"": ""Alice"",
                            ""email"": ""alice@example.com"",
                            ""user_groups"": 
                                [

                                ]
                            }}
                            ]
                        }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReserveGetUserInfoResponse apiResponse = JsonSerializer.Deserialize<QReserveGetUserInfoResponse>(response)!;

        Assert.Throws<KeyNotFoundException>(() => apiResponse.GetUserGroups());
    }

}
public class QReservePlaceBookingResponse_Should
{
    [Fact]
    public void CheckReturnedData_ReturnsTrue()
    {
        string jsonString = $@"
                            {{
                            ""Data"": 
                            
                            {{
                                ""acknowledged"": {JsonSerializer.Serialize(true)},
                                ""insertedId"": ""66a75a5a3b2c3f13601b2a43""
                            }}
                            ,
                            ""Success"":{JsonSerializer.Serialize(true)}
                            }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReservePlaceBookingResponse apiResponse = JsonSerializer.Deserialize<QReservePlaceBookingResponse>(response)!;

        bool result = apiResponse.CheckReturnedData();

        Assert.True(result);
    }
    [Fact]
    public void CheckReturnedData_ThrowsExceptionIfAcknowledgedKeyNotFound()
    {
        string expectedErrorMessage = "The booking response object was not formatted correctly. Check booking manually";
        string jsonString = $@"
                            {{
                            ""Data"": 
                            
                            {{
                                ""insertedId"": ""66a75a5a3b2c3f13601b2a43""
                            }}
                            ,
                            ""Success"":{JsonSerializer.Serialize(true)}
                            }}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement response = doc.RootElement;
        QReservePlaceBookingResponse apiResponse = JsonSerializer.Deserialize<QReservePlaceBookingResponse>(response)!;

        KeyNotFoundException error = Assert.Throws<KeyNotFoundException>(() => apiResponse.CheckReturnedData());

        Assert.Equal(expectedErrorMessage, error.Message);
    }
}