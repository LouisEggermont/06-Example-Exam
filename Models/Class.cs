namespace Howest.MCT.Models;
public class MyClass
{
    [JsonProperty("class")]
    public string ClassName { get; set; }

    [JsonProperty("meals")]
    public List<Meal> Meals { get; set; } = new List<Meal>();

    // Other properties if any
}