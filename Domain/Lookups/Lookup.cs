using Newtonsoft.Json;
using System.Collections.Generic;

namespace Domain.Lookups
{
    public class Lookup
    {
        [JsonProperty("id")]

        public const string LookupId = "08f6410a-58e9-464b-8d7a-1832bf5d7a27";
        public List<string> ModeratorIds { get; set; } = new List<string>();
        public List<string> PeopleIds { get; set; } = new List<string>();
    }
}
