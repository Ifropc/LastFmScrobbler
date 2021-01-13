using Newtonsoft.Json;

namespace LastFmScrobbler.Config
{
    public class LastFmCredentials
    {
        [JsonProperty(PropertyName = "api_key")]
        public string Key { get; set; } = null!;

        [JsonProperty(PropertyName = "secret")]
        public string Secret { get; set; } = null!;
    }
}