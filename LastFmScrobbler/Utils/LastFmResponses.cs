using Newtonsoft.Json;

namespace LastFmScrobbler.Utils
{
    public class AuthToken
    {
        [JsonProperty(PropertyName = "token")] public string Token { get; set; } = null!;
    }

    public class AuthSession
    {
        [JsonProperty(PropertyName = "name")] public string Name { get; set; } = null!;

        [JsonProperty(PropertyName = "key")] public string Key { get; set; } = null!;
    }
}