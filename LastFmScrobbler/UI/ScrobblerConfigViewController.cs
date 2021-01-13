using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using LastFmScrobbler.Config;
using LastFmScrobbler.Managers;
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

        [UIValue("colour")] private string colour = "#BD288199";

        private string _token = null!;

        public async void Initialize()
        {
            if (_lastFmClient.AuthTokenTask == null) return;

            // Wait for 3 PauseChamps and BSML
            await Utilities.PauseChamp;
            await Utilities.PauseChamp;
            await Utilities.PauseChamp;

            try
            {
                var token = await _lastFmClient.AuthTokenTask;
                ConsumeToken(token);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        [UIAction("clicked-auth-button")]
        protected void ClickedAuth()
        {
            _lastFmClient.Authorize(_token);
        }

        [UIAction("clicked-confirm-button")]
        protected void ClickedConfirm()
        {
        }

        private void ConsumeToken(string token)
        {
            _authButton.interactable = true;
            _token = token;
        }

        private void HandleException(Exception e)
        {
            _log.Error(e);
        }
    }
}