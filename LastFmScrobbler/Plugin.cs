using System;
using HarmonyLib;
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
        private const string HarmonyID = "com.github.ifropc.BSLastFmScrobbler";

        // TODO: remove if not used
        private readonly Harmony _harmony;

        private readonly Logger _log;

        [Init]
        public Plugin(IPAConfig cfg, Logger log, Zenjector injector, PluginMetadata metadata)
        {
            _log = log;
            _harmony = new Harmony(HarmonyID);
            
            var config = cfg.Generated<MainConfig>();
            config.Version = metadata.Version;

            injector.On<PCAppInit>().Pseudo(container =>
            {
                container.BindLoggerAsSiraLogger(log);
                container.BindInstance(config).AsSingle();
            });

            injector.OnMenu<Installers.MenuInstaller>();
        }


        #region Disableable

        [OnEnable]
        public void OnEnable()
        {
            try
            {
                _harmony.PatchAll();
            }
            catch (Exception e)
            {
                _log.Critical(e);
            }
        }

        [OnDisable]
        public void OnDisable()
        {
            try
            {
                _harmony.UnpatchAll(HarmonyID);
            }
            catch (Exception e)
            {
                _log.Critical(e);
            }
        }

        #endregion
    }
}