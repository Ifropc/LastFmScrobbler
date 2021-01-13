using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using Zenject;

namespace LastFmScrobbler.UI
{
    public class MenuButtonHandler: IInitializable, IDisposable
    {
        private readonly MenuButton _menuButton;
        private readonly ScrobblerFlowCoordinator _flowCoordinator;
        
        public MenuButtonHandler(ScrobblerFlowCoordinator flowCoordinator)
        {
            _flowCoordinator = flowCoordinator;
            _menuButton =  new MenuButton("Scrobbler", OnClick);
        }

        public void OnClick()
        {
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_flowCoordinator);
        }
        
        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
        }
    }
}