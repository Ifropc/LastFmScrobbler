using System;
using System.Threading;
using LastFmScrobbler.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public class LastFmManager : IInitializable
    {
        private const string BaseUrl = "http://ws.audioscrobbler.com";

        [Inject] private readonly WebClient _client;
        [Inject] private readonly SiraLog _log;

        private LastFmCredentials? _credentials;
        [Inject] private ICredentialsManager _credentialsManager;

        public void Initialize()
        {
            var loaded = _credentialsManager.LoadCredentials();

            if (loaded is not null) _credentials = JsonConvert.DeserializeObject<LastFmCredentials>(loaded);
        }

        public string? GetAuthToken()
        {
            return WithCredentials(c =>
            {
                var json = GetRequest(
                    $"{BaseUrl}/2.0/?method=auth.gettoken&api_key={c.api_key}&format=json");

                return json.GetValue("token")?.ToString();
            });
        }

        private T? WithCredentials<T>(Func<LastFmCredentials, T> f)
        {
            return _credentials is null ? default : f(_credentials);
        }

        private JObject GetRequest(string url)
        {
            var resp = _client.GetAsync(url, new CancellationToken());
            return CheckError(resp.Result.ConvertToJObject(), url);
        }

        private JObject CheckError(JObject json, string url)
        {
            var err = json.GetValue("error");

            if (err is not null)
            {
                var msg = json.GetValue("message")?.ToString();
                _log.Error($"Failed to execute request to {url}. Error code: {err}, message: {msg}");
            }

            return json;
        }
    }
}