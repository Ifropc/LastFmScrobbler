using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using Zenject;

namespace LastFmScrobbler.UI
{
    public class MenuButtonHandler : IInitializable, IDisposable
    {
        private readonly ScrobblerFlowCoordinator _flowCoordinator;
        private readonly MenuButton _menuButton;

        public MenuButtonHandler(ScrobblerFlowCoordinator flowCoordinator)
        {
            _flowCoordinator = flowCoordinator;
            _menuButton = new MenuButton("Scrobbler", OnClick);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable && BSMLParser.IsSingletonAvailable) MenuButtons.instance.UnregisterButton(_menuButton);
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void OnClick()
        {
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_flowCoordinator);
        }
    }
}