using LastFmScrobbler.Components;
using LastFmScrobbler.Config;
using LastFmScrobbler.Managers;
using LastFmScrobbler.UI;
using SiraUtil;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace LastFmScrobbler.Installers
{
    public class MenuInstaller : Installer
    {
        [Inject] private readonly SiraLog _log = null!;

        public override void InstallBindings()
        {
            InstallUI();
            InstallScrobbler();
        }

        private void InstallUI()
        {
            Container.BindViewController<ScrobblerConfigView>();
            Container.BindViewController<NotAuthorizedView>();
            Container.BindFlowCoordinator<ScrobblerFlowCoordinator>();
            Container.BindInterfacesAndSelfTo<MenuButtonHandler>().AsSingle().NonLazy();

            _log.Debug("Finished setting up UI");
        }

        private void InstallScrobbler()
        {
            var cfg = Container.Resolve<MainConfig>();

            Container.BindInterfacesAndSelfTo<CredentialsLoader>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LinksOpener>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LastFmClient>().AsSingle().NonLazy();

            if (!cfg.IsAuthorized())
            {
                _log.Warning("Client is not authorized, scrobbler is disabled.");
                return;
            }

            Rebind<MenuTransitionsHelper, LastFmMenuTransitionHelper>();
            Container.BindInterfacesAndSelfTo<SongManager>().AsSingle().NonLazy();

            _log.Info("Setup if finished.");
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