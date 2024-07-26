using System.Text.Json;
using LabBookingLib.BookingSystem;

namespace LabBookingLib.ApiResponses
{

    public class QReservePlaceBookingResponse : IAPIResponse
    {

        public bool CheckReturnedData()
        {

            if (!Data.TryGetProperty("acknowledged", out JsonElement acknowledged))
            {
                throw new KeyNotFoundException("");
            }
            else
            {
                return acknowledged.GetBoolean();
            }

        }

        // Can implement methods here to get nested data from JSON data object if required.
    }
    public class QReserveGetProjectAndRequestInfo : IAPIResponse
    {

        public string GetProjectID()
        {

            InvalidOperationException error = new("Could not get project ID for current project.");
            if (Data.ValueKind != JsonValueKind.Array)
            {
                throw error;
            }

            JsonElement.ArrayEnumerator dataEnumerator = Data.EnumerateArray();

            if (!dataEnumerator.MoveNext())
            {
                throw error;
            }

            JsonElement firstElement = dataEnumerator.Current;

            if (dataEnumerator.MoveNext())
            {
                throw error;
            }

            if (!firstElement.TryGetProperty("_id", out JsonElement nameElement))
            {
                throw error;
            }

            return nameElement.GetString()!;
        }
        public (string, string) GetMostRecentRequestInfo()
        {

            // TODO check Data value is one element array
            if (!PropertyFinder.TryGetNestedProperty(Data, "requests", out JsonElement requests))
            {
                throw new KeyNotFoundException("No requests found on that project");
            }
            if (requests.GetArrayLength() == 0)
            {
                throw new KeyNotFoundException("No requests found on that project");
            }

            foreach (JsonElement requestElement in requests.EnumerateArray())
            {
                // TODO: this only checks that the request is valid on this instrument
                // if there are multiple requests on a project it will take first that matches irrespective of the acquisition type
                // native and proteomics for example *should* be on separate projects but need to catch that as an edge case
                if (requestElement.TryGetProperty("name", out JsonElement nameElement) && QReserveConstants.RequestNames.Contains(nameElement.GetString()))
                {

                    if (requestElement.TryGetProperty("account_number", out JsonElement accountNumberElement) &&
                        requestElement.TryGetProperty("users", out JsonElement usersElement))
                    {
                        // TODO implement logic to validate account codes (specific to each institution/lab)
                        string accountNumber = accountNumberElement.GetString()!;
                        string usersId = usersElement.GetString()!;

                        return (accountNumber, usersId); // Exit the loop once the first matching request is found
                    }
                    else
                    {
                        throw new KeyNotFoundException("a valid user or account number could not be found");
                    }
                }
                else
                {
                    throw new InvalidOperationException("A valid request could not be found");
                }
            }
            throw new KeyNotFoundException("Could not find a valid request for that project");
        }
    }
    // public class QReserveGetProjectInfoResponse : IAPIResponse
    // {

    //     public (string, string) GetAccountNumberAndUserId()
    //     {

    //         if (!PropertyFinder.TryGetNestedProperty(Data, "account_number", out JsonElement accountNumber))
    //         {
    //             throw new KeyNotFoundException("Could not find account number");
    //         }
    //         if (!PropertyFinder.TryGetNestedProperty(Data, "user_id", out JsonElement user_id))
    //         {
    //             throw new KeyNotFoundException("Could not find user_id");
    //         }
    //         return (accountNumber.GetString(), user_id.GetString())!;
    //     }
    // }
    public class QReserveGetRatesByResourceResponse : IAPIResponse
    {

        public Dictionary<string, int> GetRatesDict()
        {

            if (Data.GetArrayLength() == 0)
            {
                throw new KeyNotFoundException("Could not find any rates for that resource");
            }
            Dictionary<string, int> ratesDict = [];
            foreach (JsonElement rate in Data.EnumerateArray())
            {
                if (rate.TryGetProperty("rate", out JsonElement rateValue) &&
                        rate.TryGetProperty("user_groups", out JsonElement userGroup))
                {
                    if (userGroup.TryGetProperty("_id", out JsonElement userGroupId))
                    {
                        Console.WriteLine(userGroupId.GetString()!, rateValue.GetInt16());
                        ratesDict.Add(userGroupId.GetString()!, rateValue.GetInt16());
                    }
                    else
                    {
                        throw new KeyNotFoundException("There is an error in the rate information for that resource");

                    }
                }
                else
                {
                    throw new KeyNotFoundException("There is an error in the rate information for that resource");
                }
            }
            return ratesDict;
        }
    }
    public class QReserveGetUserInfoResponse : IAPIResponse
    {
        // TODO: unsure what the structure of the rates value is - likely list of user group IDs - might need checking
        public List<string> GetUserGroups()
        {

            // redundant when beck end schema is implemented
            if (!PropertyFinder.TryGetNestedProperty(Data, "user_groups", out JsonElement userGroups))
            {
                throw new KeyNotFoundException("Could not find user groups for the user");
            }
            if (userGroups.GetArrayLength() == 0)
            {
                throw new KeyNotFoundException("Could not find any usergroups for that users");
            }
            List<string> usergroupsList = [];
            foreach (JsonElement prop in userGroups.EnumerateArray())
            {
                if (PropertyFinder.TryGetNestedProperty(prop, "_id", out JsonElement userGroupId))
                {
                    usergroupsList.Add(userGroupId.GetString()!);
                }
                else
                {
                    throw new KeyNotFoundException("Could not find user groups for the user");
                }
            }

            return usergroupsList;
        }

    }
}