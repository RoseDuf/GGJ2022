using System;
using Game;
using UnityEngine;

namespace UnityTemplateProjects.Villagers
{
    public class VillagerDayNightSensor : DayNightSensibleMonoBehaviour
    {
        private Villager _villager;

        private void Awake()
        {
            _villager = GetComponent<Villager>();
            Debug.Assert(_villager);
        }

        protected override void OnDay(int dayNumber)
        {
            _villager.UpdateStatsForDay(dayNumber);
        }

        protected override void OnNight(int nightNumber)
        {
            // Do nothing
        }

        protected override void OnDayNightTransitionStarted()
        {
            _villager.StopMoving();
        }

        protected override void OnDayNightTransitionFinished()
        {
            _villager.ResumeMoving();
        }
    }
}