namespace LabBookingLib.ApiClients;
using LabBookingLib.ApiResponses;
public interface IApiClient
{
    Task<T> PerformPostRequest<T>(string payload, string url) where T : IAPIResponse, new();
    Task<T> PerformGetRequest<T>(string url) where T : IAPIResponse, new();
}