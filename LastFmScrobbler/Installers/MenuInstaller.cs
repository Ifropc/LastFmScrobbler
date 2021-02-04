using LastFmScrobbler.Config;
using LastFmScrobbler.Managers;
using LastFmScrobbler.UI;
using SiraUtil;
using SiraUtil.Tools;
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
            Container.Bind<ScrobblerConfigView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<NotAuthorizedView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<AuthorizedView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ScrobblerFlowCoordinator>().FromNewComponentOnNewGameObject(nameof(ScrobblerFlowCoordinator)).AsSingle();
            Container.BindInterfacesAndSelfTo<MenuButtonHandler>().AsSingle();

            _log.Debug("Finished setting up UI");
        }

        private void InstallScrobbler()
        {
            var cfg = Container.Resolve<MainConfig>();

            Container.BindInterfacesAndSelfTo<CredentialsLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<LinksOpener>().AsSingle();
            Container.BindInterfacesAndSelfTo<LastFmClient>().AsSingle();

            if (!cfg.IsAuthorized())
            {
                _log.Warning("Client is not authorized, scrobbler is disabled.");
                return;
            }
            Container.BindInterfacesAndSelfTo<SongManager>().AsSingle();

            _log.Info("Setup if finished.");
        }
    }
}