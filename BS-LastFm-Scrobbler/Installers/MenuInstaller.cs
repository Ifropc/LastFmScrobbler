using BS_LastFm_Scrobbler.Components;
using BS_LastFm_Scrobbler.Managers;
using SiraUtil;
using UnityEngine;
using Zenject;

namespace BS_LastFm_Scrobbler.Installers
{
    public class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Rebind<MenuTransitionsHelper, LastFmMenuTransitionHelper>();

            Container.BindInterfacesAndSelfTo<SongManager>().AsSingle().NonLazy();
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