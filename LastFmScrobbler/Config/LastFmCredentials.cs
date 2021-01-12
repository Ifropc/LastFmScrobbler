#pragma warning disable 8618, 649
// Disables warning: fields are assigned with Zenject.

namespace LastFmScrobbler.Config
{
    public class LastFmCredentials
    {
        public string api_key { get; set; }
        public string secret { get; set; }
    }
}