namespace LabBooking.Tests.BookingSystem;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using System.Net.Sockets;
using LabBookingLib.ApiResponses;
using LabBookingLib.BookingSystem;
using LabBookingLib.ApiClients;

public class QReserveBooking_Should
{
    [Fact]
    public async Task PerformGetRequest_ReturnsDeserializedObject()
    {

        // Arrange
        var expectedResponse = $@"
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
                            ],
                            ""Success"":{JsonSerializer.Serialize(true)}
                            }}";

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedResponse)
            });
        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        QReserveApiClient api = new QReserveApiClient(httpClient);


        List<string> expected = ["669f6d6c6ee12e497ebcc794", "669f6d6c6ee12e497ebcc795"];

        // Act
        QReserveGetUserInfoResponse result = await api.PerformGetRequest<QReserveGetUserInfoResponse>("http://example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result.GetUserGroups());
    }
    [Fact]
    public async Task PerformGetRequest_ThrowsGeneralHttpRequestExcption_WhenApiEndpointIsUnreachable()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("API endpoint is unreachable"));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        QReserveApiClient api = new QReserveApiClient(httpClient);

        // Act and Assert
        ApiErrorException e = await Assert.ThrowsAsync<ApiErrorException>(async () => await api.PerformGetRequest<QReserveGetUserInfoResponse>("http://example.com"));
        Assert.NotNull(e.InnerException);
        Assert.IsType<HttpRequestException>(e.InnerException);

    }
    [Fact]
    public async Task PerformGetRequest_ThrowsHttpRequestException_WhenHostNotFound()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("API endpoint is unreachable", new SocketException((int)SocketError.HostNotFound)));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        QReserveApiClient api = new QReserveApiClient(httpClient);

        // Act and Assert
        ApiErrorException e = await Assert.ThrowsAsync<ApiErrorException>(async () => await api.PerformGetRequest<QReserveGetUserInfoResponse>("http://example.com"));


        // Additional assertions
        Assert.Equal((int)HttpStatusCode.NotFound, e.Code);
        Assert.Equal("Host not found.", e.Message);

        // Check the InnerException of the ApiErrorException
        Assert.NotNull(e.InnerException);
        Assert.IsType<HttpRequestException>(e.InnerException);

        var httpRequestException = e.InnerException as HttpRequestException;
        Assert.NotNull(httpRequestException.InnerException);
        Assert.IsType<SocketException>(httpRequestException.InnerException);

        var socketException = httpRequestException.InnerException as SocketException;
        Assert.Equal(SocketError.HostNotFound, socketException.SocketErrorCode);

    }
    [Fact]
    public async Task PerformGetRequest_ThrowsNonHttpRequestException_InvalidAddressUsed()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new ArgumentException("Invalid argument provided"));
        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        QReserveApiClient api = new QReserveApiClient(httpClient);

        // Act and Assert
        ApiErrorException e = await Assert.ThrowsAsync<ApiErrorException>(async () => await api.PerformGetRequest<QReserveGetUserInfoResponse>("http://example.com"));

        // Additional assertions
        Assert.Equal(500, e.Code);
        Assert.Equal("Unexpected error.", e.Message);
        // Check the InnerException of the ApiErrorException
        Assert.NotNull(e.InnerException);
        Assert.IsType<ArgumentException>(e.InnerException);

        var innerException = e.InnerException as ArgumentException;
        Assert.Equal("Invalid argument provided", innerException.Message);

    }

    [Fact]
    public async Task GetProjectAndRequest_ReturnsATupleOfThreeStringsGivenAValidProjectId()
    {
        // Arrange
        var mockApiClient = new Mock<IApiClient>();
        var service = new QReserve(mockApiClient.Object);

        string jsonString = $@"
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
                                ";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement jsonResponse = doc.RootElement;

        QReserveGetProjectAndRequestInfo response = new()
        {
            Success = true,
            Data = jsonResponse
        };


        mockApiClient.Setup(client => client.PerformGetRequest<QReserveGetProjectAndRequestInfo>(It.IsAny<string>()))
            .ReturnsAsync(response);

        // Act
        var result = await service.GetProjectAndRequest("24001");

        // Assert
        Assert.Equal(("669f6d6d6ee12e497ebcc7a0", "19300001", "669f6d6c6ee12e497ebcc796"), result);
    }
    [Fact]
    public async Task GetProjectAndRequest_CallsApiWithCorrectUrl()
    {
        // Arrange
        var mockApiClient = new Mock<IApiClient>();
        var service = new QReserve(mockApiClient.Object);
        var projectCode = "TestProjectCode";
        var expectedUrl = $"{QReserveConstants.ApiRoot}/projects/search?name={projectCode}";

        string jsonString = $@"
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
                                ";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement jsonResponse = doc.RootElement;

        QReserveGetProjectAndRequestInfo response = new()
        {
            Success = true,
            Data = jsonResponse
        };
        mockApiClient.Setup(client => client.PerformGetRequest<QReserveGetProjectAndRequestInfo>(expectedUrl))
            .ReturnsAsync(response);

        await service.GetProjectAndRequest(projectCode);

        mockApiClient.Verify(client => client.PerformGetRequest<QReserveGetProjectAndRequestInfo>(expectedUrl), Times.Once);
    }
    // GetResourceRates
    [Fact]
    public async Task GetResourceRates_ReturnsADictStringIntGivenValidResourceId()
    {
        // Arrange
        var mockApiClient = new Mock<IApiClient>();
        var service = new QReserve(mockApiClient.Object);

        string jsonString = $@"
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
                                ]
                                ";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement jsonResponse = doc.RootElement;

        QReserveGetRatesByResourceResponse response = new()
        {
            Success = true,
            Data = jsonResponse
        };


        mockApiClient.Setup(client => client.PerformGetRequest<QReserveGetRatesByResourceResponse>(It.IsAny<string>()))
            .ReturnsAsync(response);

        Dictionary<string, int> expected = new()
        {
            { "669f6d6c6ee12e497ebcc794", 50 }, //internal
            { "669f6d6c6ee12e497ebcc795", 100 }, //external
        };
        // Act
        var result = await service.GetResourceRates("669f6d6d6ee12e497ebcc798");

        // Assert
        Assert.Equal(expected, result);
    }
    [Fact]
    public async Task GetResourceRates_CallsApiWithCorrectUrl()
    {
        // Arrange
        var mockApiClient = new Mock<IApiClient>();
        var service = new QReserve(mockApiClient.Object);
        var resourceId = "669f6d6d6ee12e497ebcc798";
        var expectedUrl = $"{QReserveConstants.ApiRoot}/resources/{resourceId}/rates";

        string jsonString = $@"
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
                                ]
                                ";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement jsonResponse = doc.RootElement;

        QReserveGetRatesByResourceResponse response = new()
        {
            Success = true,
            Data = jsonResponse
        };
        mockApiClient.Setup(client => client.PerformGetRequest<QReserveGetRatesByResourceResponse>(expectedUrl))
            .ReturnsAsync(response);

        await service.GetResourceRates("669f6d6d6ee12e497ebcc798");

        mockApiClient.Verify(client => client.PerformGetRequest<QReserveGetRatesByResourceResponse>(expectedUrl), Times.Once);
    }

    [Theory]
    [MemberData(nameof(RatesEnums.DetermineRateIdTestData), MemberType = typeof(RatesEnums))]
    public void DetermineRateId_ReturnsAStringGivenValidInputs(Dictionary<string, int> resource, List<string> user, string expected)
    {
        StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        // Act
        string result = QReserve.DetermineRateId(resource, user);

        string output = stringWriter.ToString();
        string filePath = "..\\..\\..\\output\\property_finder_output.txt";
        File.WriteAllText(filePath, output);

        //Assert
        Assert.Equal(expected, result);
    }
    // [Fact]
    // public async Task GetResourceRates_CallsApiWithCorrectUrl()
    // {
    //     // Arrange
    //     var mockApiClient = new Mock<IApiClient>();
    //     var service = new QReserve(mockApiClient.Object);
    //     var resourceId = "669f6d6d6ee12e497ebcc798";
    //     var expectedUrl = $"{QReserveConstants.ApiRoot}/resources/{resourceId}/rates";

    //     string jsonString = $@"
    //                             [
    //                                 {{
    //                                     ""_id"": ""669f6d6d6ee12e497ebcc79a"",
    //                                     ""resources"": 
    //                                     {{
    //                                         ""_id"": ""669f6d6d6ee12e497ebcc798"",
    //                                         ""name"": ""Microscope"",
    //                                         ""type"": ""Imaging""
    //                                     }},
    //                                     ""rate"": {JsonSerializer.Serialize(50)},
    //                                     ""currency"": ""USD"",
    //                                     ""user_groups"": 
    //                                     {{
    //                                         ""_id"": ""669f6d6c6ee12e497ebcc794"",
    //                                         ""name"": ""internal""
    //                                     }}
    //                                 }},
    //                                 {{
    //                                     ""_id"": ""669f6d6d6ee12e497ebcc79b"",
    //                                     ""resources"": 
    //                                     {{
    //                                         ""_id"": ""669f6d6d6ee12e497ebcc798"",
    //                                         ""name"": ""Microscope"",
    //                                         ""type"": ""Imaging""
    //                                     }},
    //                                     ""rate"": {JsonSerializer.Serialize(100)},
    //                                     ""currency"": ""USD"",
    //                                     ""user_groups"": 
    //                                     {{
    //                                         ""_id"": ""669f6d6c6ee12e497ebcc795"",
    //                                         ""name"": ""external""
    //                                     }}
    //                                 }}
    //                             ]
    //                             ";
    //     using JsonDocument doc = JsonDocument.Parse(jsonString);
    //     JsonElement jsonResponse = doc.RootElement;

    //     QReserveGetRatesByResourceResponse response = new()
    //     {
    //         Success = true,
    //         Data = jsonResponse
    //     };
    //     mockApiClient.Setup(client => client.PerformGetRequest<QReserveGetRatesByResourceResponse>(expectedUrl))
    //         .ReturnsAsync(response);

    //     await service.GetResourceRates("669f6d6d6ee12e497ebcc798");

    //     mockApiClient.Verify(client => client.PerformGetRequest<QReserveGetRatesByResourceResponse>(expectedUrl), Times.Once);
    // }

}