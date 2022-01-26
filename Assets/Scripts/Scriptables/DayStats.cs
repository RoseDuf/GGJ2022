using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Stats/Day Stats")]
    public class DayStats : ScriptableObject
    {
        [SerializeField] private int forDay = 1;

        [SerializeField] private float baseAggressivity = 1;
        [SerializeField] private int numberOfVillagers = 10;

        public int Day => forDay;
        public float BaseAggressivity => baseAggressivity;
        public int NumberOfVillagers => numberOfVillagers;
    }
}