using UnityEngine;

namespace Game
{
    public class DayNightEnabled : DayNightSensibleMonoBehaviour
    {
        [SerializeField] private GameObject enabledObject;
        [SerializeField] private DayNight activeOn;
        
        enum DayNight { Day, Night }

        protected override void OnDay(int dayNumber)
        {
            if (enabledObject)
                enabledObject.SetActive(activeOn == DayNight.Day);
        }

        protected override void OnNight(int nightNumber)
        {
            if (enabledObject)
                enabledObject.SetActive(activeOn == DayNight.Night);
        }
    }
}