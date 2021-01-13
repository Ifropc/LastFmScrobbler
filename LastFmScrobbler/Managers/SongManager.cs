using System;
using LastFmScrobbler.Components;
using SiraUtil.Tools;
using Zenject;

namespace LastFmScrobbler.Managers
{
    public class SongManager : IInitializable, IDisposable
    {
        private Action<LevelCollectionViewController, IPreviewBeatmapLevel> _eventSelectedAction = null!;

        [Inject] private readonly LevelCollectionViewController _levelCollectionViewController = null!;
        [Inject] private readonly SiraLog _log = null!;
        [Inject] private readonly MissionSelectionMapViewController _missionSelection = null!;

        private IPreviewBeatmapLevel _selected = null!;
        [Inject] private readonly LastFmMenuTransitionHelper _transitionHelper = null!;

        public void Dispose()
        {
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
            _log.Debug($"Selected {beatmapPreview.songAuthorName}:{beatmapPreview.songName}");
        }

        private void OnMissionEventSelected(MissionSelectionMapViewController c, MissionNode n)
        {
            OnEventSelected(n.missionData.level);
        }

        private void OnLevelStarted(float offset)
        {
            _log.Debug($"Level started: {_selected.songName}:{_selected.songDuration}:{offset}");
        }

        private void OnLevelFinished(LevelCompletionResults results)
        {
            _log.Debug($"Level finished : {results.songDuration} : {results.endSongTime}");
        }
    }
}