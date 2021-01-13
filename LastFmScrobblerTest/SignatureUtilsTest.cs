using System;
using System.Collections.Generic;
using LastFmScrobbler.Utils;
using NUnit.Framework;

#pragma warning disable 8618, 649
// Disables warning: fields are assigned with Zenject.

namespace LastFmScrobblerTest
{
    [TestFixture]
    public class SignatureUtilsTest
    {
        [Test]
        public void TestSignature()
        {
            var dict = new Dictionary<string, string>
            {
                {"method", "auth.getSession"},
                {"api_key", "0123456789abcdefghijklmnopqrstuv"},
                {"token", "L_Pfc9d6Xq0bhLN6Z1ioj55kYZc5DQ5d"}
            };

            var sign = SignatureUtils.Sign(dict, "abcdefghijklmnopqrstuvwxyz012345");

            Assert.AreEqual("9E598131F4636697A22A95DBD26EC27E", sign);
        }
    }
}