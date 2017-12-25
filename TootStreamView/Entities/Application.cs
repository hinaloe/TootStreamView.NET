// original by https://github.com/amay077/MastoConsoleNetStandard
// MIT License Copyright (c) 2017 amay077


using Newtonsoft.Json;

namespace TootStreamView.Entities
{
    public class Application
    {
        /// <summary>
        /// Name of the app
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Homepage URL of the app
        /// </summary>
        [JsonProperty("website")]
        public string Website { get; set; }
    }
}