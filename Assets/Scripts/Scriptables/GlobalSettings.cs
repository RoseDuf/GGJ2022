using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Settings/Global Settings")]
    public class GlobalSettings : ScriptableObject
    {
        [Header("Points")] 
        [SerializeField] private int basePointsForKilling = 100;
        [SerializeField] private float fatnessMultiplier = 2.5f;
        [SerializeField] [Tooltip("Add this percent to the gained score for each combo")] private float comboScoreMultiplier = 0.1f;
        [SerializeField] private float secondsBeforeEndOfCombo = 5f;

        [Header("Time")] 
        [SerializeField] private float secondsOfDay = 60;
        [SerializeField] private float secondsOfNight = 30;
        [SerializeField] private bool startAtNight = false;

        public int BasePointsForKilling => basePointsForKilling;
        public float FatnessMultiplier => fatnessMultiplier;
        public float ComboScoreMultiplier => comboScoreMultiplier;
        public float SecondsBeforeEndOfCombo => secondsBeforeEndOfCombo;

        public float SecondsOfDay => secondsOfDay;
        public float SecondsOfNight => secondsOfNight;
        public bool StartAtNight => startAtNight;
    }
}