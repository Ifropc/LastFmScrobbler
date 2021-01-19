using System;
using BeatSaberMarkupLanguage.Attributes;
using LastFmScrobbler.Config;
using Zenject;

namespace LastFmScrobbler.UI
{
    [ViewDefinition("LastFmScrobbler.UI.Views.authorized-view.bsml")]
    [HotReload(RelativePathToLayout = @"\Views\authorized-view.bsml")]
    public class AuthorizedView : AbstractView
    {
        public event Action? ActionFinished;

        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly ScrobblerConfigView _configView = null!;
        [UIValue("auth-info")] public string AuthText => $"Authorized as {_config.SessionName}";

        [UIAction("clicked-logout-button")]
        protected void ClickedLogout()
        {
            _config.SessionKey = null;
            _config.SessionName = null;
            _configView.Authorized = false;
            _log.Debug("Logged out");
            ActionFinished?.Invoke();
        }
    }
}