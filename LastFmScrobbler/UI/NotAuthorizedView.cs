using System;
using System.Threading.Tasks;
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
        [UIComponent("button-auth")] public Button authButton;
        [UIComponent("button-confirm")] private Button _confirmButton;

        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly ScrobblerConfigView _configView = null!;
        [Inject] private readonly LastFmClient _lastFmClient = null!;

        public string token = null!;

        [UIAction("clicked-auth-button")]
        protected void ClickedAuth()
        {
            _lastFmClient.Authorize(token);
            _confirmButton.interactable = true;
            authButton.interactable = false;
        }

        [UIAction("clicked-confirm-button")]
        protected void ClickedConfirm()
        {
            _confirmButton.interactable = false;

            SafeAwait(_lastFmClient.GetSession(token), SessionAuthorized, () => authButton.interactable = true);
        }

        private void SessionAuthorized(AuthSession authSession)
        {
            _config.SessionKey = authSession.Key;
            _config.SessionName = authSession.Name;
            _log.Debug($"Auth confirmed for {authSession.Name}");
            _configView.Authorized = true;
        }
    }
}