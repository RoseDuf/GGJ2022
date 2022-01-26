using UnityEngine;

namespace Game
{
    public class DayNightSwap : DayNightSensibleMonoBehaviour
    {
        [SerializeField] private GameObject dayEnabledObject;
        [SerializeField] private GameObject nightEnabledObject;
        
        protected override void OnDay(int dayNumber)
        {
            if (dayEnabledObject)
                dayEnabledObject.SetActive(true);
            if (nightEnabledObject)
                nightEnabledObject.SetActive(false);
        }

        protected override void OnNight(int nightNumber)
        {
            if (dayEnabledObject)
                dayEnabledObject.SetActive(false);
            if (nightEnabledObject)
                nightEnabledObject.SetActive(true);
        }
    }
}