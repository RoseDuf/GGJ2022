using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class SettingsSystem : PersistentSingleton<SoundSystem>
    {
        private const string SETTINGS_FOLDER_PATH = "Globals";

        private GlobalSettings settings;

        public GlobalSettings Settings => settings;
        
        protected override void Awake()
        {
            base.Awake();

            LoadGlobalSettings();
        }

        private void LoadGlobalSettings()
        {
            var globalSettings = Resources.LoadAll<GlobalSettings>(SETTINGS_FOLDER_PATH);

            Debug.Assert(globalSettings.Any(), $"An object of type {nameof(GlobalSettings)} should be in the folder {SETTINGS_FOLDER_PATH}");
            if (globalSettings.Length > 1)
                Debug.LogWarning($"More than one object of type {nameof(GlobalSettings)} was found in the folder {SETTINGS_FOLDER_PATH}. Taking the first one.");

            settings = globalSettings.First();
        }
    }
}