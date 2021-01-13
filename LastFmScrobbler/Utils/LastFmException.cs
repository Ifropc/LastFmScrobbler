using System;

namespace LastFmScrobbler.Utils
{
    public class LastFmException : Exception
    {
        private const int NotAuthToken = 14;
        private const int ServiceOffline = 11;
        private const int ServerError = 16;

        public readonly string Message;
        public readonly int? ErrorCode;

        public LastFmException(string message, int? errorCode = null)
        {
            this.Message = message;
            this.ErrorCode = errorCode;
        }

        public bool ShouldBeReported()
        {
            return ErrorCode == null ||
                   ErrorCode != NotAuthToken && ErrorCode != ServerError && ErrorCode != ServiceOffline;
        }

        public bool TokenNotAuthorized()
        {
            return ErrorCode == NotAuthToken;
        }
    }
}