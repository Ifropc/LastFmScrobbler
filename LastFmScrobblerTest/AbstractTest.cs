using System;
using System.Collections.Generic;
using NUnit.Framework;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobblerTest
{
    public abstract class AbstractTest
    {
        protected DiContainer _container;
        protected List<IInitializable> _initializables;
        protected List<IDisposable> _dispozables;
        
        [SetUp]
        public void SetUp()
        {
            _container = new DiContainer();
            _initializables = new List<IInitializable>();
            _dispozables = new List<IDisposable>();
            
            _container.Bind<SiraLog>().To<MockSiraLog>().AsSingle();
            // Requred for WebClient
            _container.Bind(typeof(SiraLog).Assembly.GetType("SiraUtil.Config")).FromNew().AsSingle();
            BindInitializable<WebClient>();
            
            SetupContainer();
            
            foreach (var i in _initializables) i.Initialize();
        }

        protected abstract void SetupContainer();
        
        [TearDown]
        public void TearDown()
        {
            foreach (var i in _dispozables) i.Dispose();
        }
        
        protected void BindInitializable<T>() where T : IInitializable
        {
            _container.Bind<T>().AsSingle().NonLazy();
            _initializables.Add(_container.Resolve<T>());

            if (_container.Resolve<T>() is IDisposable disposable) _dispozables.Add(disposable);
        }
        
        internal class MockSiraLog : SiraLog
        {
        }
    }
}