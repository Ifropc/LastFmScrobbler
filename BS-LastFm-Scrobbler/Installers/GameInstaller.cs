using BS_LastFm_Scrobbler.Components;
using BS_LastFm_Scrobbler.Managers;
using SiraUtil;
using Zenject;

namespace BS_LastFm_Scrobbler.Installers
{
    public class GameInstaller : Installer
    {
        // private readonly IPA.Logging.Logger log;
        //
        // public GameInstaller(IPA.Logging.Logger log)
        // {
        //     this.log = log;
        // }

        public override void InstallBindings()
        {
            // log?.Debug("Installing bindings");
            var resolved = Container.Resolve<MenuTransitionsHelper>();
            var upgraded = resolved.Upgrade<MenuTransitionsHelper, MyMenuTransitionHelper>();
          
            Container.QueueForInject(upgraded);
            Container.Unbind<MenuTransitionsHelper>();
            Container.Bind(typeof(MenuTransitionsHelper), typeof(MyMenuTransitionHelper)).To<MyMenuTransitionHelper>().FromInstance(upgraded).AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
        }
    }
}