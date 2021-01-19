using BeatSaberMarkupLanguage;
using HMUI;
using LastFmScrobbler.Config;
using Zenject;

namespace LastFmScrobbler.UI
{
    public class ScrobblerFlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly ScrobblerConfigView _configView = null!;
        [Inject] private readonly AuthorizedView _authorizedView = null!;
        [Inject] private readonly NotAuthorizedView _notAuthorizedView = null!;
        [Inject] private readonly FadeInOutController _fadeInOutController = null!;
        [Inject] private readonly MainFlowCoordinator _mainFlowCoordinator = null!;
        [Inject] private readonly MenuTransitionsHelper _transitionHelper = null!;
        
        private bool _restartRequired;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle("LastFm Scrobbler");
                showBackButton = true;
                ProvideInitialViewControllers(_configView);
            }

            _config.OnChanged += ConfigUpdated;
            _configView.AuthClicked += ShowAuthSidePanel;
            _authorizedView.ActionFinished += HideAuthSidePanel;
            _notAuthorizedView.ActionFinished += HideAuthSidePanel;

            _configView.Initialize();
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _notAuthorizedView.ActionFinished -= HideAuthSidePanel;
            _authorizedView.ActionFinished -= HideAuthSidePanel;
            _configView.AuthClicked -= ShowAuthSidePanel;
            _config.OnChanged -= ConfigUpdated;
        }

        protected override void BackButtonWasPressed(ViewController _)
        {
            // TODO: Remove?
            if (_restartRequired)
            {
                _restartRequired = false;
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

        private void ShowAuthSidePanel(bool authorized)
        {
            SetLeftScreenViewController(
                authorized ? _authorizedView : _notAuthorizedView,
                ViewController.AnimationType.In
            );
        }

        private void HideAuthSidePanel()
        {
            SetLeftScreenViewController(null, ViewController.AnimationType.Out);
        }
    }
}