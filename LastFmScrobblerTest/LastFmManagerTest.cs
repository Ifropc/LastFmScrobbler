using System;
using System.Collections.Generic;
using System.IO;
using LastFmScrobbler.Config;
using LastFmScrobbler.Managers;
using Newtonsoft.Json;
using NUnit.Framework;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobblerTest
{
    [TestFixture]
    public class LastFmManagerTest : AbstractTest
    {
        protected override void SetupContainer()
        {
            _container.Bind<ICredentialsManager>().To<TestCredentialsManager>().AsSingle();

            BindInitializable<LastFmManager>();
        }

        [Test]
        public void TestGetRequest()
        {
            var m = _container.Resolve<LastFmManager>();

            var tok = m.GetAuthToken();

            Console.Write($"Got token: {tok}");

            Assert.IsNotEmpty(tok);
        }
    }

    [TestFixture]
    public class LastFmManagerTestFailedCreds : AbstractTest
    {
        protected override void SetupContainer()
        {
            _container.Bind<ICredentialsManager>().To<TestCredentialsManagerFailedCreds>().AsSingle();

            BindInitializable<LastFmManager>();
        }

        [Test]
        public void TestFailGetCreds()
        {
            var m = _container.Resolve<LastFmManager>();

            var tok = m.GetAuthToken();

            Console.Write($"Got token: {tok}");

            Assert.IsNull(tok);
        }
    }

    internal class TestCredentialsManagerFailedCreds : TestCredentialsManager
    {
        public override LastFmCredentials? LoadCredentials()
        {
            var c = base.LoadCredentials();
            c!.api_key += "123";
            return c;
        }
    }

    internal class TestCredentialsManager : ICredentialsManager
    {
        public virtual LastFmCredentials? LoadCredentials()
        {
            var t = File.ReadAllText("../../../../LastFmScrobbler/credentials.txt");

            return JsonConvert.DeserializeObject<LastFmCredentials>(t);
        }
    }
}