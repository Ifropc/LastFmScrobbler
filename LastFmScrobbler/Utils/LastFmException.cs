using System;

namespace LastFmScrobbler.Utils
{
    public class LastFmException : Exception
    {
        private const int NotAuthToken = 14;
        private const int ServiceOffline = 11;
        private const int ServerError = 16;
        private readonly int? _errorCode;

        public LastFmException(string message, int? errorCode = null) : base(message)
        {
            _errorCode = errorCode;
        }

        public bool ShouldBeReported()
        {
            return _errorCode == null ||
                   _errorCode != NotAuthToken && _errorCode != ServerError && _errorCode != ServiceOffline;
        }

        public bool TokenNotAuthorized()
        {
            return _errorCode == NotAuthToken;
        }
    }
}