using IPA;
using IPA.Config.Stores;
using IPAConfig = IPA.Config.Config;
using IPA.Loader;
using SiraUtil.Zenject;
using UnityEngine;

namespace BS_LastFm_Scrobbler
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        [Init]
        public Plugin(IPAConfig cfg, IPA.Logging.Logger log, Zenjector injector, PluginMetadata metadata)
        {
            var config = cfg.Generated<Config>();
            config.Version = metadata.Version;

            injector.On<PCAppInit>().Pseudo(container =>
            {
                log?.Debug("On app init");
            });
        }

        // TODO

        #region Disableable

        [OnEnable]
        public void OnEnable()
        {
        }

        [OnDisable]
        public void OnDisable()
        {
        }

        #endregion
    }
}