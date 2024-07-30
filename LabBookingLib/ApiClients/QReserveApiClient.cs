
namespace LabBookingLib.ApiClients;
using LabBookingLib.ApiResponses;
using System.Text;
using System.Text.Json;

public class QReserveApiClient : IApiClient
{
    private readonly HttpClient httpClient;

    public QReserveApiClient() : this(new HttpClient())
    {

    }
    public QReserveApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<T> PerformGetRequest<T>(string url) where T : IAPIResponse, new()
    {
        try
        {

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // request.Headers.Add("Authorization", "QR-1jb8b9fhlghoz3klpiwt8e847bmqoxwlox1y3ipxz7rlz0meuf7zj4q4j5f402wt57owu3917z4");

            using HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            // TODO: need to further handle null if deserialisation fails - create reponse object with bad status and a null error

            T? apiResponse = JsonSerializer.Deserialize<T>(responseBody) ?? throw new Exception("Failed to deserialize the response.");
            return apiResponse;
        }
        catch (HttpRequestException e) when (e.InnerException is System.Net.Sockets.SocketException se && se.SocketErrorCode == System.Net.Sockets.SocketError.HostNotFound)
        {
            throw new ApiErrorException((int)System.Net.HttpStatusCode.NotFound, "Host not found.", e);
        }
        catch (HttpRequestException e)
        {
            throw new ApiErrorException(e.StatusCode.HasValue ? (int)e.StatusCode.Value : 0, "Http Request error. There was a problem contacting the booking server.", e);
        }
        catch (Exception e)
        {
            throw new ApiErrorException(500, "Unexpected error.", e);
        }
    }

    public async Task<T> PerformPostRequest<T>(string payload, string url) where T : IAPIResponse, new()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };
            // request.Headers.Add("Authorization", "QR-1jb8b9fhlghoz3klpiwt8e847bmqoxwlox1y3ipxz7rlz0meuf7zj4q4j5f402wt57owu3917z4");

            HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            // TODO: need to further handle null if deserialisation fails - create reponse object with bad status and a null error
            T? apiResponse = JsonSerializer.Deserialize<T>(responseBody) ?? throw new Exception("Failed to deserialize the response.");

            return apiResponse;
        }
        catch (HttpRequestException e) when (e.InnerException is System.Net.Sockets.SocketException se && se.SocketErrorCode == System.Net.Sockets.SocketError.HostNotFound)
        {
            throw new ApiErrorException((int)System.Net.HttpStatusCode.NotFound, "Host not found.", e);
        }
        catch (HttpRequestException e)
        {
            throw new ApiErrorException(e.StatusCode.HasValue ? (int)e.StatusCode.Value : 0, "Request error.", e);
        }
        catch (Exception e)
        {
            throw new ApiErrorException(500, "Unexpected error.", e);
        }
    }
}