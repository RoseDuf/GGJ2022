using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class DayStatsSystem : PersistentSingleton<DayStatsSystem>
    {
        private const string DAY_STATS_FOLDER_PATH = "DayStats";

        private List<DayStats> dayStats = new List<DayStats>();
        
        protected override void Awake()
        {
            base.Awake();

            LoadDayStats();
        }

        private void LoadDayStats()
        {
            dayStats = Resources.LoadAll<DayStats>(DAY_STATS_FOLDER_PATH).ToList();

            Debug.Assert(dayStats.Any(), $"At least one object of type {nameof(DayStats)} should be in the folder {DAY_STATS_FOLDER_PATH}");

            dayStats.Sort((statLeft, statRight) => statLeft.Day.CompareTo(statRight.Day));
        }
        
        public DayStats GetForDay(int dayNumber)
        {
            var foundDayStats = dayStats.First();

            foreach (var stats in dayStats)
            {
                if (stats.Day <= dayNumber)
                {
                    foundDayStats = stats;
                }
            }

            return foundDayStats;
        }
    }
}