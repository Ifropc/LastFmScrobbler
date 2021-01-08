using System;
using HarmonyLib;
using SiraUtil.Tools;
using Zenject;

namespace BS_LastFm_Scrobbler.Patches
{
    [HarmonyPatch(typeof(MenuTransitionsHelper))]
    public class TransitionHelperPatch
    {
        private static SiraLog _log;
        public static event Action LevelStarted;
        
        public TransitionHelperPatch(SiraLog log)
        {
            log?.Debug("Helper installed");
            _log = log;
        }

        [HarmonyPrefix]
        [HarmonyPatch(
            "StartStandardLevel",
            typeof(string),
            typeof(IDifficultyBeatmap),
            typeof(OverrideEnvironmentSettings),
            typeof(ColorScheme),
            typeof(GameplayModifiers),
            typeof(PlayerSpecificSettings),
            typeof(PracticeSettings),
            typeof(string),
            typeof(bool),
            typeof(Action),
            typeof(Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults>)
        )]
        static void Prefix(
            string gameMode,
            IDifficultyBeatmap difficultyBeatmap,
            OverrideEnvironmentSettings overrideEnvironmentSettings,
            ColorScheme overrideColorScheme,
            GameplayModifiers gameplayModifiers,
            PlayerSpecificSettings playerSpecificSettings,
            PracticeSettings practiceSettings,
            string backButtonText,
            bool useTestNoteCutSoundEffects,
            Action beforeSceneSwitchCallback,
            Action<StandardLevelScenesTransitionSetupDataSO, LevelCompletionResults> levelFinishedCallback)
        {
            LevelStarted?.Invoke();
        
            _log.Debug("Hello from Sira Log!");
        }
    }
}