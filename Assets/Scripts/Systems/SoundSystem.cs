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
        public void PlaySplashSound() => events.SplashEvent.Post(gameObject);
        public void PlayBadEndingSound() => events.BadEndingDeathEvent.Post(gameObject);
        public void PlayGoodEndingSound() => events.GoodEndingEscapeEvent.Post(gameObject);
        public void PlayGiveFoodSound(GameObject player) => events.GiveFoodEvent.Post(player);
        public void PlayPickupFoodSound(GameObject player) => events.PickupFoodEvent.Post(player);
        public void PlayVillagerAttackSound(GameObject villager) => events.VillagerAttackEvent.Post(villager);
        public void PlayVillagerDeathSound(GameObject villager) => events.VillagerDeathEvent.Post(villager);
        public void PlayVillagerJoySound(GameObject villager) => events.VillagerJoyEvent.Post(villager);
        public void PlayDogStepSound(GameObject player) => events.DogStepEvent.Post(player);
        public void PlayWolfHurtSound(GameObject player) => events.WolfHurtEvent.Post(player);
        public void PlayWolfStepSound(GameObject player) => events.WolfStepsEvent.Post(player);
        public void PlayWolfAttackSound(GameObject player) => events.WolfAttackEvent.Post(player);
        
        public void PlayMenuMusic() => events.MenuMusicEvent.Post(gameObject);
        public void PlayDayMusic() => events.DayMusicEvent.Post(gameObject);
        public void PlayEveningMusic() => events.EveningMusicEvent.Post(gameObject);
        public void PlayNightMusic() => events.NightMusicEvent.Post(gameObject);

        public void SetMasterVolume(float volume) => events.MasterVolumeParam.SetValue(gameObject, volume * 100);
        public void SetMusicVolume(float volume) => events.MusicVolumeParam.SetValue(gameObject, volume * 100);
        public void SetSfxVolume(float volume) => events.SfxVolumeParam.SetValue(gameObject, volume * 100);
        public float GetMasterVolume() => events.MasterVolumeParam.GetValue(gameObject) / 100f;
        public float GetMusicVolume() => events.MusicVolumeParam.GetValue(gameObject) / 100f;
        public float GetSfxVolume() => events.SfxVolumeParam.GetValue(gameObject) / 100f;

    }
}