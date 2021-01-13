using System;
using BeatSaberMarkupLanguage;
using HMUI;
using LastFmScrobbler.Components;
using LastFmScrobbler.Config;
using Zenject;

namespace LastFmScrobbler.UI
{
    public class ScrobblerFlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly ScrobblerConfigViewController _configViewController = null!;
        [Inject] private readonly FadeInOutController _fadeInOutController = null!;
        [Inject] private readonly MenuTransitionsHelper _transitionHelper = null!;
        [Inject] private readonly MainFlowCoordinator _mainFlowCoordinator = null!;

        private bool _restartRequired;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!firstActivation) return;
            
            SetTitle("LastFm Scrobbler");
            showBackButton = true;
            ProvideInitialViewControllers(_configViewController);
            
            _configViewController.Initialize();

            _config.OnChanged += ConfigUpdated;
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _config.OnChanged -= ConfigUpdated;
        }

        protected override void BackButtonWasPressed(ViewController _)
        {
            if (_restartRequired)
            {
                _fadeInOutController.FadeOut(0.35f, () => _transitionHelper.RestartGame());
            }
            else
            {
                _mainFlowCoordinator.DismissFlowCoordinator(this);
            }
        }

        private void ConfigUpdated()
        {
            _restartRequired = true;
        }
    }
}