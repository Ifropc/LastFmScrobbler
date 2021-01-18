using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using LastFmScrobbler.Config;
using LastFmScrobbler.Managers;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LastFmScrobblerTest
{
    [TestFixture]
    public class LastFmManagerTest : AbstractTest
    {
        protected override void SetupContainer()
        {
            _container.Bind<ICredentialsLoader>().To<TestCredentialsLoader>().AsSingle();
            _container.Bind<ILinksOpener>().To<TestLinksOpener>().AsSingle();

            BindInitializable<LastFmClient>();
        }

        [Test]
        public void TestAuthorize()
        {
            var client = _container.Resolve<LastFmClient>();

            var t = client.AuthTokenTask!;

            Assert.NotNull(t);
            t!.Wait();
            Assert.IsNotNull(t.Result);
            Console.Write(t.Result);
        }

        [Test]
        public void TestCurrentlyPlaying()
        {
            var client = _container.Resolve<LastFmClient>();

            var t = client.SendNowPlaying("Yes", "Roundabout", 510)!;

            Assert.NotNull(t);
            t!.Wait();
            Assert.IsNotNull(t.IsCompletedSuccessfully);
            Console.Write(t.Result);
        }

        [Test]
        public void TestCurrentlyPlayingUrlUnsafeSymbols()
        {
            var client = _container.Resolve<LastFmClient>();

            var t = client.SendNowPlaying("t+pazolite", "trick or die?", 510)!;

            Assert.NotNull(t);
            t!.Wait();
            Assert.IsNotNull(t.IsCompletedSuccessfully);
            Console.Write(t.Result);
        }

        [Test]
        public void TestScrobble()
        {
            var client = _container.Resolve<LastFmClient>();

            // Test non-english symbols
            var t = client.SendScrobble("DJ Krush", "若輩", 271)!;

            Assert.NotNull(t);
            t!.Wait();
            Assert.IsNotNull(t.IsCompletedSuccessfully);
            Console.Write(t.Result.Scrobbles.Attribute.Accepted);
        }
    }

    [TestFixture]
    public class LastFmManagerTestFailedCreds : AbstractTest
    {
        protected override void SetupContainer()
        {
            _container.Bind<ICredentialsLoader>().To<TestCredentialsLoaderFailedCreds>().AsSingle();
            _container.Bind<ILinksOpener>().To<TestLinksOpener>().AsSingle();

            BindInitializable<LastFmClient>();
        }

        [Test]
        public void TestFailGetCreds()
        {
            var client = _container.Resolve<LastFmClient>();

            var t = client.AuthTokenTask;

            Assert.NotNull(t);
            Assert.Throws<AggregateException>(t!.Wait);
        }
    }

    internal class TestCredentialsLoaderFailedCreds : TestCredentialsLoader
    {
        public override LastFmCredentials? LoadCredentials()
        {
            var c = base.LoadCredentials();
            c!.Key += "123";
            return c;
        }
    }

    internal class TestCredentialsLoader : ICredentialsLoader
    {
        public virtual LastFmCredentials? LoadCredentials()
        {
            var t = File.ReadAllText("../../../../LastFmScrobbler/credentials.json");

            return JsonConvert.DeserializeObject<LastFmCredentials>(t);
        }
    }

    internal class TestLinksOpener : ILinksOpener
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