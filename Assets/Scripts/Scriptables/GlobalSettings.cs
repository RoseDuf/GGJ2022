using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Settings/Global Settings")]
    public class GlobalSettings : ScriptableObject
    {
        [Header("Points")] 
        [SerializeField] private int pointsForSmallGuy = 100;
        [SerializeField] private int pointsForBigGuy = 250;
        [SerializeField] [Tooltip("Add this percent to the gained score for each combo")] private float comboScoreMultiplier = 0.1f;

        public int PointsForSmallGuy => pointsForSmallGuy;
        public int PointsForBigGuy => pointsForBigGuy;
        public float ComboScoreMultiplier => comboScoreMultiplier;
    }
}