using System;
using System.Threading;
using System.Threading.Tasks;
using LastFmScrobbler.Config;
using Newtonsoft.Json.Linq;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

#pragma warning disable 8618, 649
// Disables warning: fields are assigned with Zenject.

namespace LastFmScrobbler.Managers
{
    public class LastFmManager : IInitializable
    {
        private const string ScrobblerBaseUrl = "http://ws.audioscrobbler.com";
        private const string LastFmBaseUrl = "http://www.last.fm/api";

        [Inject] private WebClient _client;

        private LastFmCredentials? _credentials;
        public string? authToken { get; private set; }
        [Inject] private ICredentialsManager _credentialsManager;
        [Inject] private ILinksManager _linksManager;
        [Inject] private SiraLog _log;

        public void Initialize()
        {
            _credentials = _credentialsManager.LoadCredentials();
        }

        public Task? Authorize()
        {
            return WithCredentials(c =>
            {
                _log.Debug("Sending auth request");

                return GetRequest(
                    $"{ScrobblerBaseUrl}/2.0/?method=auth.gettoken&api_key={c.api_key}&format=json",
                    tokenTask =>
                    {
                        var json = CheckError(tokenTask.Result.ConvertToJObject());

                        authToken = json.GetValue("token")!.ToString();

                        var url = $"{LastFmBaseUrl}/auth/?api_key={c.api_key}&token={authToken}";

                        _linksManager.OpenLink(url);
                    });
            });
        }

        private T? WithCredentials<T>(Func<LastFmCredentials, T> f)
        {
            if (_credentials is null) return default;

            try
            {
                return f(_credentials);
            }
            catch (Exception e)
            {
                _log.Error(e);
                return default;
            }
        }

        private Task GetRequest(string url, Action<Task<WebResponse>> a)
        {
            var resp = _client.GetAsync(url, new CancellationToken());

            return resp.ContinueWith(a);
        }

        private static JObject CheckError(JObject json)
        {
            var err = json.GetValue("error");

            if (err is null) return json;

            var msg = json.GetValue("message")?.ToString();
            throw new Exception($"Failed to execute request. Error code: {err}, message: {msg}");
        }
    }
}