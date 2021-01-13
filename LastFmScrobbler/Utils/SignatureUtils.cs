using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#pragma warning disable 8618, 649
// Disables warning: fields are assigned with Zenject.

namespace LastFmScrobbler.Utils
{
    public class SignatureUtils
    {
        public static string Sign(Dictionary<string, string> parameters, string secret)
        {
            var md5 = MD5.Create();

            var result = "";

            foreach (var (key, value) in parameters.OrderBy(p => p.Key))
            {
                result += key;
                result += value;
            }

            result += secret;

            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(result));

            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}