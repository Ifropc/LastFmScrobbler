using System;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using LastFmScrobbler.Config;
using LastFmScrobbler.Managers;
using LastFmScrobbler.Utils;
using SiraUtil;
using SiraUtil.Tools;
using UnityEngine.UI;
using Zenject;

namespace LastFmScrobbler.UI
{
    [ViewDefinition("LastFmScrobbler.UI.Views.config-view.bsml")]
    [HotReload(RelativePathToLayout = @"\Views\ConfigView.bsml")]
    public class ScrobblerConfigViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly LastFmClient _lastFmClient = null!;
        [Inject] private readonly SiraLog _log = null!;

        [UIComponent("button-auth")] private Button _authButton;
        [UIComponent("button-confirm")] private Button _confirmButton;

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

        [UIValue("auth-text")] public string AuthText => Authorized ? "Authorized" : "Not authorized";
        [UIValue("auth-color")] public string AuthColor => Authorized ? "#baf2bb" : "#BD288199";

        private string _token = null!;

        public async void Initialize()
        {
            if (_lastFmClient.AuthTokenTask == null) return;

            // Wait for 3 PauseChamps and BSML
            await Utilities.PauseChamp;
            await Utilities.PauseChamp;
            await Utilities.PauseChamp;

            SafeAwait(_lastFmClient.AuthTokenTask, ConsumeToken);
        }

        [UIAction("clicked-auth-button")]
        protected void ClickedAuth()
        {
            _lastFmClient.Authorize(_token);
            _confirmButton.interactable = true;
            _authButton.interactable = false;
        }

        [UIAction("clicked-confirm-button")]
        protected async void ClickedConfirm()
        {
            _confirmButton.interactable = false;

            SafeAwait(_lastFmClient.GetSession(_token), SessionAuthorized, () => _authButton.interactable = true);
        }

        private async void SafeAwait<T>(Task<T> task, Action<T> onSuccess, Action? onError = null)
        {
            try
            {
                onSuccess(await task);
            }
            catch (Exception e)
            {
                HandleException(e);
                onError?.Invoke();
            }
        }

        private void ConsumeToken(string token)
        {
            _authButton.interactable = true;
            _token = token;
        }

        private void SessionAuthorized(AuthSession authSession)
        {
            _config.SessionKey = authSession.Key;
            _config.SessionName = authSession.Name;
            _log.Debug($"Auth confirmed for {authSession.Name}");
            Authorized = true;
        }

        private void HandleException(Exception e)
        {
            _log.Error(e);
        }
    }
}