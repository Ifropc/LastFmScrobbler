using System;
using System.Collections.Generic;
using System.IO;
using LastFmScrobbler.Managers;
using NUnit.Framework;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobblerTest
{
    [TestFixture]
    public class UtilsTests
    {
        [SetUp]
        public void SetUp()
        {
            _container = new DiContainer();

            _initializables = new List<IInitializable>();
            _dispozables = new List<IDisposable>();

            // Requred for WebClient
            _container.Bind(typeof(SiraLog).Assembly.GetType("SiraUtil.Config")).FromNew().AsSingle();
            _container.Bind<SiraLog>().To<MockSiraLog>().AsSingle();
            _container.Bind<ICredentialsManager>().To<TestCredentialsManager>().AsSingle();

            BindInitializable<WebClient>();
            BindInitializable<LastFmManager>();


            foreach (var i in _initializables) i.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var i in _dispozables) i.Dispose();
        }

        private List<IInitializable> _initializables;
        private List<IDisposable> _dispozables;
        private DiContainer _container;

        private void BindInitializable<T>() where T : IInitializable
        {
            _container.Bind<T>().AsSingle().NonLazy();
            _initializables.Add(_container.Resolve<T>());

            if (_container.Resolve<T>() is IDisposable disposable) _dispozables.Add(disposable);
        }

        [Test]
        public void TestGetRequest()
        {
            var m = _container.Resolve<LastFmManager>();

            Console.Write(m.GetAuthToken());
        }
    }

    internal class MockSiraLog : SiraLog
    {
    }

    internal class TestCredentialsManager : ICredentialsManager
    {
        public string LoadCredentials()
        {
            return File.ReadAllText("../../../../LastFmScrobbler/credentials.txt");
        }
    }
}