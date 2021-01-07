using System;
using BS_LastFm_Scrobbler.Components;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace BS_LastFm_Scrobbler.Managers
{
    public class GameManager : IInitializable, IDisposable
    {
        [Inject] public MyMenuTransitionHelper TransitionHelper;


        [Inject] private readonly SiraLog _log = null;


        public void Initialize()
        {
            _log.Debug("Initialized");
            TransitionHelper.SongDidStartEvent += OnSongStarted;
        }

        public void Dispose()
        {
            TransitionHelper.SongDidStartEvent -= OnSongStarted;
        }

        private void OnSongStarted(IDifficultyBeatmap beatmap)
        {
            var characteristic = beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic;
            _log.Debug(
                $"On song started : {characteristic.serializedName} {characteristic.descriptionLocalizationKey}  {characteristic.characteristicNameLocalizationKey} {characteristic.compoundIdPartName}");
        }

        private void OnSongEnded()
        {
        }
    }
}
