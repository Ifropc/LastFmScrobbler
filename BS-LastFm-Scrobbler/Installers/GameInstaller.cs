using BS_LastFm_Scrobbler.Components;
using BS_LastFm_Scrobbler.Managers;
using BS_LastFm_Scrobbler.Patches;
using SiraUtil;
using UnityEngine;
using Zenject;

namespace BS_LastFm_Scrobbler.Installers
{
    public class GameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Rebind<MenuTransitionsHelper, MyMenuTransitionHelper>();

            // TODO: might be not necessary
            Container.Bind<TransitionHelperPatch>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SongManager>().AsSingle().NonLazy();
        }

        // TODO: delete if unused
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