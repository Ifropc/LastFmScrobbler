using System;
using System.IO;
using System.Reflection;
using LastFmScrobbler.Config;
using Newtonsoft.Json;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public interface ICredentialsLoader
    {
        public LastFmCredentials LoadCredentials();
    }

    public class CredentialsLoader : ICredentialsLoader
    {
        private const string CredentialsLocation = "LastFmScrobbler.credentials.json";

        [Inject] private readonly SiraLog _log = null!;

        public LastFmCredentials LoadCredentials()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(CredentialsLocation) ??
                               throw new Exception("Failed to load last fm credentials");
            using var reader = new StreamReader(stream);
            var credentials = JsonConvert.DeserializeObject<LastFmCredentials>(reader.ReadToEnd());
            _log.Debug("Credentials loaded");
            return credentials;
        }
    }
}