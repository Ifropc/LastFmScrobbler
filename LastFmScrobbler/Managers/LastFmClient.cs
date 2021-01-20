using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LastFmScrobbler.Config;
using LastFmScrobbler.Utils;
using Newtonsoft.Json.Linq;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public class LastFmClient : IInitializable
    {
        private const string ScrobblerBaseUrl = "https://ws.audioscrobbler.com/2.0/";
        private const string LastFmBaseUrl = "https://www.last.fm/api";
        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly ICredentialsLoader _credentialsLoader = null!;
        [Inject] private readonly ILinksOpener _linksOpener = null!;
        [Inject] private readonly SiraLog _log = null!;

        private HttpClient? _client;

        private LastFmCredentials _credentials = null!;

        public Task<string>? AuthTokenTask { get; private set; }

        public void Initialize()
        {
            _client ??= new HttpClient();

            _credentials = _credentialsLoader.LoadCredentials();

            if (!_config.IsAuthorized()) AuthTokenTask = GetToken();
        }

        public async Task<string> GetToken()
        {
            _log.Debug("Sending token request");

            var url = $"{ScrobblerBaseUrl}?method=auth.gettoken&api_key={_credentials.Key}&format=json";

            var resp = await _client!.GetStringAsync(url);

            _log.Debug("Got response for token request");

            return CheckError<AuthToken>(resp).Token;
        }

        public void Authorize(string authToken)
        {
            _log.Debug("Sending auth request");

            var url = $"{LastFmBaseUrl}/auth/?api_key={_credentials.Key}&token={authToken!}";

            _linksOpener.OpenLink(url);
        }

        public async Task<AuthSession> GetSession(string token)
        {
            var parameters = new Dictionary<string, string>
            {
                {"method", "auth.getSession"},
                {"api_key", _credentials.Key},
                {"token", token}
            };

            var url = $"{ScrobblerBaseUrl}?{SignatureUtils.SignedParams(parameters, _credentials.Secret)}";

            var resp = await _client!.GetStringAsync(url);

            _log.Debug($"Got response for auth request {resp}");

            return CheckError<AuthSessionResponse>(resp).Session;
        }

        // Return object only for testing purpose 
        public async Task<object> SendNowPlaying(string artist, string track, int duration)
        {
            var parameters = new Dictionary<string, string>
            {
                {"method", "track.updateNowPlaying"},
                {"artist", artist},
                {"track", track},
                {"duration", duration.ToString()},
                {"api_key", _credentials.Key},
                {"sk", _config.SessionKey!}
            };

            var resp = await PostAsync(parameters);

            _log.Debug($"Got response for update now request {resp}");

            return CheckError<object>(resp);
        }

        // Return object only for testing purpose 
        public async Task<ScrobbleResponse> SendScrobble(string artist, string track, int duration)
        {
            var timestamp = DateTime.Now.ToUnixTime();

            var parameters = new Dictionary<string, string>
            {
                {"method", "track.scrobble"},
                {"artist", artist},
                {"track", track},
                {"timestamp", timestamp.ToString()},
                {"duration", duration.ToString()},
                {"api_key", _credentials.Key},
                {"sk", _config.SessionKey!}
            };

            var resp = await PostAsync(parameters);

            _log.Debug($"Got response for scrobble request {resp}");

            return CheckError<ScrobbleResponse>(resp);
        }

        private async Task<string> PostAsync(Dictionary<string, string> parameters)
        {
            var body = SignatureUtils.SignedParams(parameters, _credentials.Secret);

            var data = new StringContent(body, Encoding.UTF8, null);

            var httpResponse = await _client!.PostAsync(ScrobblerBaseUrl, data);

            return await httpResponse.Content.ReadAsStringAsync();
        }

        private static T CheckError<T>(string resp)
        {
            var json = JObject.Parse(resp);

            var err = json.GetValue("error");

            if (err is null) return json.ToObject<T>() ?? throw new LastFmException($"Failed to deserialize {json}");

            var msg = json.GetValue("message")?.ToString();

            throw new LastFmException(msg ?? "<Unknown http error>", err.ToObject<int>());
        }
    }
}