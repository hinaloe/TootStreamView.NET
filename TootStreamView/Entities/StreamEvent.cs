using Newtonsoft.Json;

namespace TootStreamView.Entities
{
    public class StreamEvent
    {
        /// <summary>
        /// The event type
        /// </summary>
        [JsonProperty("event")]
        public string Event { get; set; }

        /// <summary>
        /// Event payload text
        /// </summary>
        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}