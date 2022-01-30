using System.Linq;
using UnityEngine;

namespace Game
{
    public class SoundSystem : PersistentSingleton<SoundSystem>
    {
        private const string SOUNDS_FOLDER_PATH = "Globals";

        private SoundEvents events;
        
        protected override void Awake()
        {
            base.Awake();

            LoadSoundEvents();
            LoadBank();
        }

        private void LoadSoundEvents()
        {
            var soundEvents = Resources.LoadAll<SoundEvents>(SOUNDS_FOLDER_PATH);

            Debug.Assert(soundEvents.Any(), $"An object of type {nameof(SoundEvents)} should be in the folder {SOUNDS_FOLDER_PATH}");
            if (soundEvents.Length > 1)
                Debug.LogWarning($"More than one object of type {nameof(SoundEvents)} was found in the folder {SOUNDS_FOLDER_PATH}. Taking the first one.");

            events = soundEvents.First();
        }
        
        private void LoadBank()
        {
            Instantiate(events.SoundBankPrefab, transform);
        }

        public void PlayBababooeySound() => events.BababooeyEvent.Post(gameObject);
        public void PlayMenuMusic() => events.MenuMusicEvent.Post(gameObject);
        public void PlayDayMusic() => events.DayMusicEvent.Post(gameObject);
        public void PlayNightMusic() => events.NightMusicEvent.Post(gameObject);
    }
}