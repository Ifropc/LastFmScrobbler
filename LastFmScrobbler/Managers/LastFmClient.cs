using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LastFmScrobbler.Config;
using LastFmScrobbler.Utils;
using Newtonsoft.Json.Linq;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public class LastFmClient : IInitializable
    {
        private const string ScrobblerBaseUrl = "http://ws.audioscrobbler.com/2.0/?";
        private const string LastFmBaseUrl = "http://www.last.fm/api";

        [Inject] private readonly WebClient _client = null!;
        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly ICredentialsLoader _credentialsLoader = null!;
        [Inject] private readonly ILinksOpener _linksOpener = null!;
        [Inject] private readonly SiraLog _log = null!;

        private LastFmCredentials _credentials = null!;

        public Task<string>? AuthTokenTask { get; private set; }

        public void Initialize()
        {
            _credentials = _credentialsLoader.LoadCredentials();

            if (!_config.IsAuthorized()) AuthTokenTask = GetToken();
        }

        private async Task<string> GetToken()
        {
            _log.Debug("Sending token request");

            var url = $"{ScrobblerBaseUrl}method=auth.gettoken&api_key={_credentials.Key}&format=json";

            var resp = await _client.GetAsync(url, new CancellationToken());

            _log.Debug("Got response for token request");

            return CheckError<AuthToken>(resp.ConvertToJObject()).Token;
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
            
            var url = $"{ScrobblerBaseUrl}{SignatureUtils.SignedParams(parameters, _credentials.Secret)}";
            
            var resp = await _client.GetAsync(url, new CancellationToken());
            
            _log.Debug("Got response for auth request");

            return CheckError<AuthSession>(resp.ConvertToJObject());
        }

        private static T CheckError<T>(JObject json)
        {
            var err = json.GetValue("error");

            if (err is null) return json.ToObject<T>() ?? throw new LastFmException($"Failed to deserialize {json}");

            var msg = json.GetValue("message")?.ToString();

            throw new LastFmException(msg ?? "<Unknown http error>", err.ToObject<int>());
        }
    }
}