using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using IPA.Logging;
using LastFmScrobbler.Config;
using SiraUtil;
using SiraUtil.Zenject;
using IPAConfig = IPA.Config.Config;

namespace LastFmScrobbler
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        private readonly Logger _log;

        [Init]
        public Plugin(IPAConfig cfg, Logger log, Zenjector injector, PluginMetadata metadata)
        {
            _log = log;

            var config = cfg.Generated<MainConfig>();
            config.Version = metadata.Version;

            injector.On<PCAppInit>().Pseudo(container =>
            {
                container.BindLoggerAsSiraLogger(log);
                container.BindInstance(config).AsSingle();
            });

            injector.OnMenu<Installers.MenuInstaller>();

            _log.Debug("Finished plugin initialization");
        }

        [OnEnable]
        [OnDisable]
        public void Nop()
        {
        }
    }
}