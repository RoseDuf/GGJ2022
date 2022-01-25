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
        
        [Header("Soundtrack")]
        [SerializeField] private AK.Wwise.Event menuMusic;

        public GameObject SoundBankPrefab => soundBankPrefab;
        public AK.Wwise.Event BababooeyEvent => bababooey;
        public AK.Wwise.Event MenuMusicEvent => menuMusic;
    }
}
