using System.Linq;
using BS_LastFm_Scrobbler.Components;
using BS_LastFm_Scrobbler.Installers;
using BS_LastFm_Scrobbler.Managers;
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
        [Init]
        public Plugin(IPAConfig cfg, IPA.Logging.Logger log, Zenjector injector, PluginMetadata metadata)
        {
            // var config = cfg.Generated<Config>();
            // config.Version = metadata.Version;

            injector.On<PCAppInit>().Pseudo(container =>
            {
                log?.Debug("On app init");
                container.BindLoggerAsSiraLogger(log);
                // container.BindInstance(config).AsSingle();
            });

         //   injector.OnMenu<GameInstaller>();

            injector
                .On<MenuInstaller>()
                .Pseudo((ctx, Container) =>
                {
                    var resolved = Container.Resolve<MenuTransitionsHelper>();
                    var upgraded = resolved.Upgrade<MenuTransitionsHelper, MyMenuTransitionHelper>();
          
                    Container.QueueForInject(upgraded);
                    Container.Unbind<MenuTransitionsHelper>();
                    Container.Bind(typeof(MenuTransitionsHelper), typeof(MyMenuTransitionHelper)).To<MyMenuTransitionHelper>().FromInstance(upgraded).AsSingle();
            
                    Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
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