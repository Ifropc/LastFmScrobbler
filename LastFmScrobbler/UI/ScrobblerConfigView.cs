using System;
using BeatSaberMarkupLanguage.Attributes;
using LastFmScrobbler.Config;
using LastFmScrobbler.Managers;
using Zenject;

namespace LastFmScrobbler.UI
{
    [ViewDefinition("LastFmScrobbler.UI.Views.config-view.bsml")]
    [HotReload(RelativePathToLayout = @"\Views\config-view.bsml")]
    public class ScrobblerConfigView : AbstractView
    {
        public event Action<bool>? AuthClicked;

        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly ILinksOpener _linksOpener = null!;

        private bool _authorized;

        [UIValue("authorized")]
        public bool Authorized
        {
            get => _authorized;
            set
            {
                _authorized = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(AuthText));
                NotifyPropertyChanged(nameof(AuthColor));
            }
        }

        [UIValue("scrobble-enable")]
        public bool ScrobbleEnabled
        {
            get => _config.ScrobbleEnabled;
            set
            {
                // Without it BSML initialization would trigger _restartRequired and I don't want it.
                if (_config.ScrobbleEnabled != value)
                {
                    _config.ScrobbleEnabled = value;
                }
            }
        }

        [UIValue("scrobble-percentage")]
        public int ScrobblePercentage
        {
            get => _config.SongScrobbleLength;
            set
            {
                if (_config.SongScrobbleLength != value)
                {
                    _config.SongScrobbleLength = value;
                }
            }
        }

        [UIValue("auth-text")]
        public string AuthText => Authorized ? $"Logged in as {_config.SessionName}" : "Not authorized";

        [UIValue("auth-color")] public string AuthColor => Authorized ? "#018b01" : "#8b0101";

        public void Initialize()
        {
            Authorized = _config.IsAuthorized();
        }

        [UIAction("clicked-show-auth-button")]
        protected void ClickedShow()
        {
            _log.Debug("Auth clicked");
            AuthClicked?.Invoke(Authorized);
        }

        [UIAction("clicked-github")]
        protected void ClickedGithub()
        {
            ShowInfoModal(() => _linksOpener.OpenLink("https://github.com/Ifropc/LastFmScrobbler"));
        }
    }
}