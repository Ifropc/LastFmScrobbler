using System;
using LastFmScrobbler.Components;
using LastFmScrobbler.Config;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public class SongManager : IInitializable, IDisposable
    {
        [Inject] private readonly LastFmClient _client = null!;
        [Inject] private readonly MainConfig _config = null!;
        [Inject] private readonly LevelCollectionViewController _levelCollectionViewController = null!;
        [Inject] private readonly SiraLog _log = null!;
        [Inject] private readonly MissionSelectionMapViewController _missionSelection = null!;
        [Inject] private readonly LastFmMenuTransitionHelper _transitionHelper = null!;

        private Action<LevelCollectionViewController, IPreviewBeatmapLevel> _eventSelectedAction = null!;
        private IPreviewBeatmapLevel _selected = null!;
        private CurrentSongData? _songData;

        public void Dispose()
        {
            _transitionHelper.SongFinishedEvent += OnLevelFinished;
            _transitionHelper.SongStartedEvent += OnLevelStarted;
            _transitionHelper.SongSelectedEvent += OnEventSelected;
            _missionSelection.didSelectMissionLevelEvent += OnMissionEventSelected;
            _levelCollectionViewController.didSelectLevelEvent -= _eventSelectedAction;
        }

        public void Initialize()
        {
            _eventSelectedAction = (_, x) => OnEventSelected(x);

            _levelCollectionViewController.didSelectLevelEvent += _eventSelectedAction;
            _missionSelection.didSelectMissionLevelEvent += OnMissionEventSelected;
            _transitionHelper.SongSelectedEvent += OnEventSelected;
            _transitionHelper.SongStartedEvent += OnLevelStarted;
            _transitionHelper.SongFinishedEvent += OnLevelFinished;
        }

        private void OnEventSelected(IPreviewBeatmapLevel beatmapPreview)
        {
            _selected = beatmapPreview;
        }

        private void OnMissionEventSelected(MissionSelectionMapViewController c, MissionNode n)
        {
            OnEventSelected(n.missionData.level);
        }

        // For 2 methods below check https://www.last.fm/api/scrobbling for more info
        private async void OnLevelStarted(float offset)
        {
            var shouldBeScrobbled = _selected.songDuration.TotalSeconds() > 30;
            var time = DateTime.Now.ToUnixTime();

            if (string.IsNullOrEmpty(_selected.songAuthorName))
            {
                shouldBeScrobbled = false;
                _log.Debug("Skipping song with empty author name");
            }
            else
            {
                try
                {
                    if (_config.NowPlayingEnabled)
                    {
                        await _client.SendNowPlaying(
                            _selected.songAuthorName,
                            _selected.songName,
                            _selected.songDuration.Seconds()
                        );
                    }
                }
                catch (Exception e)
                {
                    _log.Warning(
                        $"Failed to send now playing: {_selected.songAuthorName} - {_selected.songAuthorName}");
                    _log.Warning(e);
                }
            }

            _songData = new CurrentSongData(offset, shouldBeScrobbled, time);
        }

        private async void OnLevelFinished(LevelCompletionResults results)
        {
            var toScrobble = _songData;

            if (toScrobble is null)
            {
                _log.Warning("Unexpected null in song data");
                return;
            }

            _songData = null;

            var notEnoughPlayed = (results.endSongTime - toScrobble.Offset) / _selected.songDuration <
                                  _config.SongScrobbleLength / 100d;

            if (!toScrobble.ShouldBeScrobbled || notEnoughPlayed) return;

            try
            {
                var res = await _client.SendScrobble(
                    _selected.songAuthorName,
                    _selected.songName,
                    _selected.songDuration.Seconds(),
                    toScrobble.StartTimestamp
                );

                if (res.Scrobbles.Attribute.Accepted != 1)
                {
                    var ignoredMessage = res.Scrobbles.Data.IgnoredMessage;
                    _log.Warning($"Scrobble was rejected with code: {ignoredMessage.Code}, message: {ignoredMessage.Text}");
                    // TODO: cache failing scrobbles and re-submit them later 
                }
            }
            catch (Exception e)
            {
                _log.Warning($"Failed to scrobble: {_selected.songAuthorName} - {_selected.songAuthorName}");
                _log.Warning(e);
            }
        }

        private class CurrentSongData
        {
            internal readonly float Offset;
            internal readonly bool ShouldBeScrobbled;
            internal readonly long StartTimestamp;

            internal CurrentSongData(float offset, bool shouldBeScrobbled, long startTimestamp)
            {
                ShouldBeScrobbled = shouldBeScrobbled;
                StartTimestamp = startTimestamp;
                Offset = offset;
            }
        }
    }
}