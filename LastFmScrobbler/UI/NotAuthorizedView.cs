using System;
using BeatSaberMarkupLanguage.Attributes;
using LastFmScrobbler.Config;
using LastFmScrobbler.Managers;
using LastFmScrobbler.Utils;
using UnityEngine.UI;
using Zenject;

namespace LastFmScrobbler.UI
{
    [ViewDefinition("LastFmScrobbler.UI.Views.not-authorized-view.bsml")]
    [HotReload(RelativePathToLayout = @"\Views\not-authorized-view.bsml")]
    public class NotAuthorizedView : AbstractView
    {
        public event Action? ActionFinished;

        [UIComponent("button-auth")] public Button authButton = null!;
        [UIComponent("button-confirm")] public Button confirmButton = null!;

        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly ScrobblerConfigView _configView = null!;
        [Inject] private readonly LastFmClient _lastFmClient = null!;

        private string? _token;

        [UIAction("clicked-auth-button")]
        protected void ClickedAuth()
        {
            authButton.interactable = false;

            if (_token == null)
            {
                SafeAwait(_lastFmClient.GetToken(), Authorize, () => authButton.interactable = true);
            }
            else
            {
                Authorize(_token);
            }
        }

        private void Authorize(string authToken)
        {
            authButton.interactable = true;
            _token = authToken;
            ShowInfoModal(() =>
            {
                authButton.interactable = false;
                _lastFmClient.Authorize(authToken);
                confirmButton.interactable = true;
            });
        }

        [UIAction("clicked-confirm-button")]
        protected void ClickedConfirm()
        {
            confirmButton.interactable = false;

            SafeAwait(_lastFmClient.GetSession(_token!), SessionAuthorized);

            authButton.interactable = true;
        }

        private void SessionAuthorized(AuthSession authSession)
        {
            _config.SessionKey = authSession.Key;
            _config.SessionName = authSession.Name;
            _configView.Authorized = true;
            _token = null;
            _log.Debug($"Auth confirmed for {authSession.Name}");
            ActionFinished?.Invoke();
        }
    }
}