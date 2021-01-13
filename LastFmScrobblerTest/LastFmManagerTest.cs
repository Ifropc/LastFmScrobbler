using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
            _container.Bind<ILinksManager>().To<TestLinksManager>().AsSingle();

            BindInitializable<LastFmManager>();
        }

        [Test]
        public void TestAuthorize()
        {
            var m = _container.Resolve<LastFmManager>();

            var t = m.Authorize();
            
            Assert.NotNull(t);
            t!.Wait();
            Assert.IsNotNull(m.authToken);
            Console.Write(m.authToken);
        }
    }

    [TestFixture]
    public class LastFmManagerTestFailedCreds : AbstractTest
    {
        protected override void SetupContainer()
        {
            _container.Bind<ICredentialsManager>().To<TestCredentialsManagerFailedCreds>().AsSingle();
            _container.Bind<ILinksManager>().To<TestLinksManager>().AsSingle();

            BindInitializable<LastFmManager>();
        }

        [Test]
        public void TestFailGetCreds()
        {
            var m = _container.Resolve<LastFmManager>();

            var t = m.Authorize();
            
            Assert.NotNull(t);
            Assert.Throws<AggregateException>(t!.Wait);
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

    internal class TestLinksManager : ILinksManager
    {
        public void OpenLink(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}