namespace Howest.MCT.Models;
    public class Child
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "class")]
        public string Class { get; set; }

        [JsonProperty(PropertyName = "parentEmailAddresses")]
        public List<string> ParentEmailAddresses { get; set; }

    public override string ToString()
    {
        return $"id: {Id}";
    }
}
