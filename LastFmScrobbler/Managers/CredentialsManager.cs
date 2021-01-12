using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public interface ICredentialsManager
    {
        
        public string? LoadCredentials();
    }

    public class CredentialsManager : ICredentialsManager
    {
        private const string CredentialsLocation = "LastFmScrobbler.credentials.txt";
        
        [Inject] private SiraLog _log;

        public string? LoadCredentials()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream(CredentialsLocation) ??
                                   throw new Exception("Failed to load last fm credentials");
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
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