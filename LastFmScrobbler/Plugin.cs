using System;
using HarmonyLib;
using IPA;
using IPA.Logging;
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
        public Plugin(IPAConfig cfg, Logger log, Zenjector injector)
        {
            _log = log;
            _harmony = new Harmony(HarmonyID);

            injector.On<PCAppInit>().Pseudo(container => { container.BindLoggerAsSiraLogger(log); });

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