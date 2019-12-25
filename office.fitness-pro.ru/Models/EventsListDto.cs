using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace office.fitness_pro.ru.Models
{
    public class EventsDto
    {
        [JsonProperty("id")]
        public Guid Guid { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("eventForm")]
        public Description EventForm { get; set; }
        [JsonProperty("eventPartTime")]
        public Description EventPartTime { get; set; }
        [JsonProperty("eventType")]
        public Description EventType { get; set; }
        [JsonProperty("department")]
        public Description Department { get; set; }
    }

    public class Description
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
