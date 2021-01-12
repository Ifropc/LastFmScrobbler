using System;
using System.IO;
using System.Reflection;
using LastFmScrobbler.Config;
using Newtonsoft.Json;
using SiraUtil.Tools;
using Zenject;

#pragma warning disable 8618, 649
// Disables warning: fields are assigned with Zenject.

namespace LastFmScrobbler.Managers
{
    public interface ICredentialsManager
    {
        public LastFmCredentials? LoadCredentials();
    }

    public class CredentialsManager : ICredentialsManager
    {
        private const string CredentialsLocation = "LastFmScrobbler.credentials.txt";

        [Inject] private SiraLog _log;

        public LastFmCredentials? LoadCredentials()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream(CredentialsLocation) ??
                                   throw new Exception("Failed to load last fm credentials");
                using var reader = new StreamReader(stream);
                return JsonConvert.DeserializeObject<LastFmCredentials>(reader.ReadToEnd());
            }
            catch (Exception e)
            {
                _log.Error($"Failed to load last fm credentials: {e.Message}");
                _log.Debug(e);
                return null;
            }
        }
    }
}