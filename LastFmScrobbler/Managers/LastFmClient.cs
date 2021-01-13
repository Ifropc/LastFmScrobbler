using System;
using System.Threading;
using System.Threading.Tasks;
using LastFmScrobbler.Config;
using Newtonsoft.Json.Linq;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public class LastFmClient : IInitializable
    {
        private const string ScrobblerBaseUrl = "http://ws.audioscrobbler.com";
        private const string LastFmBaseUrl = "http://www.last.fm/api";

        [Inject] private readonly WebClient _client = null!;

        private LastFmCredentials? _credentials;
        public string? AuthToken { get; private set; }
        [Inject] private readonly ICredentialsLoader _credentialsLoader = null!;
        [Inject] private readonly ILinksOpener _linksOpener = null!;
        [Inject] private readonly SiraLog _log = null!;

        public void Initialize()
        {
            _credentials = _credentialsLoader.LoadCredentials();
        }

        public Task? Authorize()
        {
            return WithCredentials(c =>
            {
                _log.Debug("Sending auth request");

                return GetRequest(
                    $"{ScrobblerBaseUrl}/2.0/?method=auth.gettoken&api_key={c.Key}&format=json",
                    tokenTask =>
                    {
                        var json = CheckError(tokenTask.Result.ConvertToJObject());

                        AuthToken = json.GetValue("token")!.ToString();

                        var url = $"{LastFmBaseUrl}/auth/?api_key={c.Key}&token={AuthToken}";

                        _linksOpener.OpenLink(url);
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