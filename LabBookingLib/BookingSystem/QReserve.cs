using System.Text;
using System.Text.Json;
using LabBookingLib.ApiResponses;
using LabBookingLib.ApiClients;

namespace LabBookingLib.BookingSystem
{
    public static class QReserveConstants
    {
        // could these be readonly and api'd on initialisation
        public const string EclipseIdentifier = "669e6fb71582c7ce4675a654";
        public static readonly List<string> Hierarchy =
        [
                 "669f6d6c6ee12e497ebcc795", // external
                "669f6d6c6ee12e497ebcc794", // internal
        ];
        public const string ApiRoot = "http://localhost:5000";

        public static readonly string[] RequestNames = ["Proteomics Request"];


    }
    public class QReserve : IBookingSystem
    {
        private readonly IApiClient _apiClient;

        public QReserve(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> MakeBookingAsync(string projectCode, DateTime startTime, double bookingDurationMinutes, string resourceIdentifier)
        {

            // get project identifier
            (string projectIdentifier, string accountNumber, string userId) = await GetProjectAndRequest(projectCode);

            Dictionary<string, int> resourceRates = await GetResourceRates(resourceIdentifier);
            string rateId = await GetRateId(userId, resourceRates);
            // make post request, sent
            Dictionary<string, object> bookingPayload = new()
            {
                {"resources", resourceIdentifier},
                {"users", userId},
                {"rates", rateId},
                {"start", startTime},
                {"duration", bookingDurationMinutes},
                {"project", projectIdentifier},
                {"account_code", accountNumber}
            };


            string payload = JsonSerializer.Serialize(bookingPayload);
            string url = $"{QReserveConstants.ApiRoot}/bookings";
            QReservePlaceBookingResponse result = await _apiClient.PerformPostRequest<QReservePlaceBookingResponse>(payload, url);
            return result.CheckReturnedData();
        }
        public async Task<string> GetRateId(string userId, Dictionary<string, int> resourceRates)
        {
            string userUrl = String.Format($"{QReserveConstants.ApiRoot}/users/{userId}");
            QReserveGetUserInfoResponse result = await _apiClient.PerformGetRequest<QReserveGetUserInfoResponse>(userUrl);

            string rateId = DetermineRateId(resourceRates, result.GetUserGroups());

            return rateId;
        }
        public static string DetermineRateId(Dictionary<string, int> resourceRate, List<string> usersGroupIds)
        {
            // TODO need to get only the usergroups that are common to the user and the resrouce rate dict first
            // then find the best one
            List<string> resourceUsergroups = [.. resourceRate.Keys];
            List<string> commonUserGroups = resourceUsergroups.Intersect(usersGroupIds).ToList();
            List<int> mappedList = commonUserGroups.Select(item => QReserveConstants.Hierarchy.IndexOf(item))
                                                .Where(index => index != -1)
                                                .ToList();
            Console.WriteLine(mappedList);
            if (mappedList.Count == 0)
            {
                throw new Exception("Charge rate could not be determined, check user and resource usergroups");
            }
            int indexOfHighestPriority = mappedList.Min();
            return QReserveConstants.Hierarchy[indexOfHighestPriority];

        }
        public async Task<Dictionary<string, int>> GetResourceRates(string resourceIdentifier)
        {
            string reservableUrl = String.Format($"{QReserveConstants.ApiRoot}/resources/{resourceIdentifier}/rates");
            QReserveGetRatesByResourceResponse result = await _apiClient.PerformGetRequest<QReserveGetRatesByResourceResponse>(reservableUrl);
            return result.GetRatesDict();
        }
        public async Task<(string, string, string)> GetProjectAndRequest(string projectCode)
        {

            string getProjectByCodeUrl = String.Format($"{QReserveConstants.ApiRoot}/projects/search?name={projectCode}");
            QReserveGetProjectAndRequestInfo response = await _apiClient.PerformGetRequest<QReserveGetProjectAndRequestInfo>(getProjectByCodeUrl);
            (string accountNumber, string userId) = response.GetMostRecentRequestInfo();
            string projectId = response.GetProjectID();
            return (projectId, accountNumber, userId);
        }




    }
}