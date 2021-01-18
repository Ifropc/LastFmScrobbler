using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace LastFmScrobbler.Utils
{
    public class SignatureUtils
    {
        public static string SignedParams(Dictionary<string, string> parameters, string secret)
        {
            var builder = new StringBuilder();

            foreach (var (key, value) in parameters)
                builder.Append(key).Append('=').Append(WebUtility.UrlEncode(value)).Append('&');

            string signature = Sign(parameters, secret);

            builder.Append("api_sig=").Append(signature).Append("&format=json");

            return builder.ToString();
        }

        public static string Sign(Dictionary<string, string> parameters, string secret)
        {
            var md5 = MD5.Create();

            var builder = new StringBuilder();

            foreach (var (key, value) in parameters.OrderBy(p => p.Key)) builder.Append(key).Append(value);

            builder.Append(secret);

            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));

            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}