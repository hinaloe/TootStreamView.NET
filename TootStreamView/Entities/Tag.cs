// original by https://github.com/amay077/MastoConsoleNetStandard
// MIT License Copyright (c) 2017 amay077

using Newtonsoft.Json;

namespace TootStreamView.Entities
{
    public class Tag
    {
        /// <summary>
        /// The hashtag, not including the preceding #
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The URL of the hashtag
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}