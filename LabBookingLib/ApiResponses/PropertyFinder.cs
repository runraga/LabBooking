using System.Text.Json;

namespace LabBookingLib.ApiResponses;

public static class PropertyFinder
{
    public static bool TryGetNestedProperty(this JsonElement element, string key, out JsonElement value)
    {

        value = default;

        if (element.ValueKind == JsonValueKind.Object)
        {
            //check for key at top level
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (property.NameEquals(key))
                {
                    value = property.Value;
                    return true;
                }
            }
            //check subsequent levels (fully and recursively)
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    if (property.Value.TryGetNestedProperty(key, out value))
                    {
                        return true;
                    }
                }

                // for arrays only check the first element
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    if (property.Value.GetArrayLength() > 0)
                    {
                        JsonElement firstElement = property.Value[0];
                        if (firstElement.TryGetNestedProperty(key, out value))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        // if top level of Data is array, take first element and search for key recursively
        if (element.ValueKind == JsonValueKind.Array)
        {
            if (element.GetArrayLength() > 0)
            {
                JsonElement firstElement = element[0];
                if (firstElement.TryGetNestedProperty(key, out value))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
