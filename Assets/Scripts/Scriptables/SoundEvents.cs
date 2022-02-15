using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Sound/Sound Events")]
    public class SoundEvents : ScriptableObject
    {
        [Header("Soundbank")] 
        [SerializeField] private GameObject soundBankPrefab;
        
        [Header("SFX")]
        [SerializeField] private AK.Wwise.Event bababooey;
        [SerializeField] private AK.Wwise.Event splash;
        
        [SerializeField] private AK.Wwise.Event goodEndingEscape;
        [SerializeField] private AK.Wwise.Event badEndingDeath;
        
        [SerializeField] private AK.Wwise.Event villagerAttack;
        [SerializeField] private AK.Wwise.Event villagerDeath;
        [SerializeField] private AK.Wwise.Event villagerJoy;
        
        [SerializeField] private AK.Wwise.Event giveFood;
        [SerializeField] private AK.Wwise.Event pickupFood;
        
        [SerializeField] private AK.Wwise.Event dogStep;
        [SerializeField] private AK.Wwise.Event wolfHurt;
        [SerializeField] private AK.Wwise.Event wolfSteps;
        [SerializeField] private AK.Wwise.Event wolfAttack;
        
        [Header("Soundtrack")]
        [SerializeField] private AK.Wwise.Event menuMusic;
        [SerializeField] private AK.Wwise.Event dayMusic;
        [SerializeField] private AK.Wwise.Event eveningMusic;
        [SerializeField] private AK.Wwise.Event nightMusic;

        [Header("RTPC")] 
        [SerializeField] private AK.Wwise.RTPC masterVolume;
        [SerializeField] private AK.Wwise.RTPC musicVolume;
        [SerializeField] private AK.Wwise.RTPC sfxVolume;

        public GameObject SoundBankPrefab => soundBankPrefab;
        
        public AK.Wwise.Event BababooeyEvent => bababooey;
        public AK.Wwise.Event SplashEvent => splash;
        public AK.Wwise.Event BadEndingDeathEvent => badEndingDeath;
        public AK.Wwise.Event GiveFoodEvent => giveFood;
        public AK.Wwise.Event GoodEndingEscapeEvent => goodEndingEscape;
        public AK.Wwise.Event PickupFoodEvent => pickupFood;
        public AK.Wwise.Event VillagerAttackEvent => villagerAttack;
        public AK.Wwise.Event VillagerDeathEvent => villagerDeath;
        public AK.Wwise.Event VillagerJoyEvent => villagerJoy;
        public AK.Wwise.Event DogStepEvent => dogStep;
        public AK.Wwise.Event WolfHurtEvent => wolfHurt;
        public AK.Wwise.Event WolfStepsEvent => wolfSteps;
        public AK.Wwise.Event WolfAttackEvent => wolfAttack;

        public AK.Wwise.RTPC MasterVolumeParam => masterVolume;
        public AK.Wwise.RTPC MusicVolumeParam => musicVolume;
        public AK.Wwise.RTPC SfxVolumeParam => sfxVolume;
        
        public AK.Wwise.Event MenuMusicEvent => menuMusic;
        public AK.Wwise.Event DayMusicEvent => dayMusic;
        public AK.Wwise.Event EveningMusicEvent => eveningMusic;
        public AK.Wwise.Event NightMusicEvent => nightMusic;
    }
}
