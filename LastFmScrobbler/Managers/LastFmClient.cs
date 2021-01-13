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
        private const string ScrobblerBaseUrl = "http://ws.audioscrobbler.com";
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

            var url = $"{ScrobblerBaseUrl}/2.0/?method=auth.gettoken&api_key={_credentials.Key}&format=json";

            var resp = await _client.GetAsync(url, new CancellationToken());

            _log.Debug("Got response for token request");

            var json = CheckError(resp.ConvertToJObject());

            return json.GetValue("token")!.ToString();
        }

        public void Authorize(string authToken)
        {
            _log.Debug("Sending auth request");

            var url = $"{LastFmBaseUrl}/auth/?api_key={_credentials.Key}&token={authToken!}";

            _linksOpener.OpenLink(url);
        }

        private static JObject CheckError(JObject json)
        {
            var err = json.GetValue("error");

            if (err is null) return json;

            var msg = json.GetValue("message")?.ToString();

            throw new LastFmException(msg ?? "<Unknown http error>", err.ToObject<int>());
        }
    }
}