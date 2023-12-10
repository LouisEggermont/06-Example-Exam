namespace Howest.MCT.Models;
using Newtonsoft.Json.Converters;

    public class Meal
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "childId")]
        public string ChildId { get; set; }

        [JsonProperty(PropertyName = "choice")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MealChoice Choice { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }
    }

public enum MealChoice
{
    HotMeal,
    Sandwiches,
    NotEating
}