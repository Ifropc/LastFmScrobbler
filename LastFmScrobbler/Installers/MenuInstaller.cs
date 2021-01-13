using System;
using LastFmScrobbler.Components;
using LastFmScrobbler.Managers;
using SiraUtil;
using UnityEngine;
using Zenject;

namespace LastFmScrobbler.Installers
{
    public class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Rebind<MenuTransitionsHelper, LastFmMenuTransitionHelper>();

            Container.BindInterfacesAndSelfTo<CredentialsManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LinksManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LastFmManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SongManager>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<TestHttpManager>().AsSingle().NonLazy();
        }

        private void Rebind<T, R>() where T : MonoBehaviour where R : T
        {
            var resolved = Container.Resolve<T>();
            var upgraded = resolved.Upgrade<T, R>();

            Container.QueueForInject(upgraded);
            Container.Unbind<T>();
            Container.Bind(typeof(T), typeof(R)).To<R>().FromInstance(upgraded).AsSingle();
        }
    }
}