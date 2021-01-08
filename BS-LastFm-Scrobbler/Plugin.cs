using System;
using System.Linq;
using BS_LastFm_Scrobbler.Components;
using BS_LastFm_Scrobbler.Installers;
using BS_LastFm_Scrobbler.Managers;
using BS_LastFm_Scrobbler.Patches;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPAConfig = IPA.Config.Config;
using IPA.Loader;
using SiraUtil;
using SiraUtil.Zenject;
using UnityEngine;
using Zenject;

namespace BS_LastFm_Scrobbler
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        private const string HarmonyID = "com.github.ifropc.BSLastFmScrobbler";
        private readonly IPA.Logging.Logger _log;
        private readonly Harmony _harmony;

        [Init]
        public Plugin(IPAConfig cfg, IPA.Logging.Logger log, Zenjector injector, PluginMetadata metadata)
        {
            _log = log;
            _harmony = new Harmony(HarmonyID);
            HarmonyLog.Log = log;

            injector.On<PCAppInit>().Pseudo(container =>
            {
                log?.Debug("On app init");
                container.BindLoggerAsSiraLogger(log);
            });

            injector.OnMenu<GameInstaller>();
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