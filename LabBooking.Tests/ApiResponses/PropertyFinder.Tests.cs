namespace LabBooking.Tests.ApiResponses;

using System.Text.Json;
using LabBookingLib.ApiResponses;

public class ApiResponses_IsPropertyFinderShould
{
    // return true if key present
    [Fact]
    public void TryGetNestedProperty_ShouldFindTopLevelKey()
    {
        //arrange
        string jsonString = "{\"name\": \"Peter\"}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement topLevelElement = doc.RootElement;

        //Act
        bool keyFound = PropertyFinder.TryGetNestedProperty(topLevelElement, "name", out JsonElement value);

        //Assert
        Assert.True(keyFound);
        Assert.Equal("Peter", value.GetString());
    }

    // return false if key not present
    [Fact]
    public void TryGetNestedProperty_ShouldReturnFalseWhenKeyMissinWhenOnlyTopLevelKeysPresent()
    {
        //arrange
        string jsonString = "{\"otherKey\": \"a value\"}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement topLevelElement = doc.RootElement;

        //Act
        bool keyFound = PropertyFinder.TryGetNestedProperty(topLevelElement, "name", out JsonElement value);

        //Assert
        Assert.False(keyFound);
        Assert.Equal(JsonValueKind.Undefined, value.ValueKind);
    }
    // should check nested elements for key
    // return true if present
    [Fact]
    public void TryGetNestedProperty_ShouldFindKeyWhenNested()
    {
        //arrange
        string jsonString = "{\"topLevelKey\": {\"name\" : \"Peter\"}}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement topLevelElement = doc.RootElement;

        //Act
        bool keyFound = PropertyFinder.TryGetNestedProperty(topLevelElement, "name", out JsonElement value);

        //Assert
        Assert.True(keyFound);
        Assert.Equal("Peter", value.GetString());
    }
    // return true if present
    [Fact]
    public void TryGetNestedProperty_ShouldReturnFalseIfKeyNotInNestedObjects()
    {
        //arrange
        string jsonString = "{\"topLevelKey\": {\"notName\" : \"Peter\"}}";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement topLevelElement = doc.RootElement;

        //Act
        bool keyFound = PropertyFinder.TryGetNestedProperty(topLevelElement, "name", out JsonElement value);

        //Assert
        Assert.False(keyFound);
        Assert.Equal(JsonValueKind.Undefined, value.ValueKind);
    }

    // should check if element is array and only check first element in array
    [Fact]
    public void TryGetNestedProperty_ShouldReturnCorrectlyIfObjectIsArray()
    {
        //arrange
        string jsonString = "[{\"topLevelKey\": {\"name\" : \"Peter\"}}]";
        using JsonDocument doc = JsonDocument.Parse(jsonString);
        JsonElement topLevelElement = doc.RootElement;

        //Act
        bool keyFound = PropertyFinder.TryGetNestedProperty(topLevelElement, "name", out JsonElement value);

        //Assert
        Assert.True(keyFound);
        Assert.Equal("Peter", value.ToString());
    }
    [Fact]
    public void TryGetNestedProperty_ShouldOnlyCheckFirstObjectInArray()
    {
        // StringWriter stringWriter = new StringWriter();
        // Console.WriteLine("this is a test");
        // Console.SetOut(stringWriter);

        //arrange - key in first element
        string jsonStringTrue = "[{\"topLevelKey\": {\"name\" : \"Peter\"}},{\"topLevelKey\": \"notNested\"}]";
        using JsonDocument docTrue = JsonDocument.Parse(jsonStringTrue);
        JsonElement topLevelElementTrue = docTrue.RootElement;
        //Act - key in first element
        bool keyFoundTrue = PropertyFinder.TryGetNestedProperty(topLevelElementTrue, "name", out JsonElement valueTrue);

        //arrange - key only in second element
        string jsonStringFalse = "[{\"topLevelKey\": \"notNested\"},{\"topLevelKey\": {\"name\" : \"Peter\"}}]";
        using JsonDocument docFalse = JsonDocument.Parse(jsonStringFalse);
        JsonElement topLevelElementFalse = docFalse.RootElement;
        //Act - key only in second element
        bool keyFoundFalse = PropertyFinder.TryGetNestedProperty(topLevelElementFalse, "name", out JsonElement valueFalse);

        // string output = stringWriter.ToString();
        // string filePath = "..\\..\\..\\output\\property_finder_output.txt";
        // File.WriteAllText(filePath, output);

        //Assert
        Assert.True(keyFoundTrue);
        Assert.Equal("Peter", valueTrue.ToString());
        Assert.False(keyFoundFalse);
        Assert.Equal(JsonValueKind.Undefined, valueFalse.ValueKind);
    }
    [Fact]
    public void TryGetNestedProperty_InNestedArraysOnlyFirstElementIsChecked()
    {


        //arrange - key in first element
        string jsonStringTrue = "{\"nestedArray\" :[{\"topLevelKey\": {\"name\" : \"Peter\"}},{\"topLevelKey\": \"notNested\"}]}";
        using JsonDocument docTrue = JsonDocument.Parse(jsonStringTrue);
        JsonElement topLevelElementTrue = docTrue.RootElement;
        //Act - key in first element
        bool keyFoundTrue = PropertyFinder.TryGetNestedProperty(topLevelElementTrue, "name", out JsonElement valueTrue);

        //arrange - key only in second element
        string jsonStringFalse = "{\"nestedArray\" :[{\"topLevelKey\": \"notNested\"},{\"topLevelKey\": {\"name\" : \"Peter\"}}]}";
        using JsonDocument docFalse = JsonDocument.Parse(jsonStringFalse);
        JsonElement topLevelElementFalse = docFalse.RootElement;
        //Act - key only in second element
        bool keyFoundFalse = PropertyFinder.TryGetNestedProperty(topLevelElementFalse, "name", out JsonElement valueFalse);

        //Assert
        Assert.True(keyFoundTrue);
        Assert.Equal("Peter", valueTrue.ToString());
        Assert.False(keyFoundFalse);
        Assert.Equal(JsonValueKind.Undefined, valueFalse.ValueKind);
    }

}