using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    public class PostProcessingManager : Singleton<PostProcessingManager>
    {
        [SerializeField] private AnimationCurve dayEveningCurve;
        
        [SerializeField] private Volume dayVolume;
        [SerializeField] private Volume eveningVolume;
        [SerializeField] private Volume nightVolume;

        private bool isDay = true;

        private void Start()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnDayStarted += OnDay;
                GameManager.Instance.OnNightStarted += OnNight;
            }
        }
        
        protected override void OnDestroy()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnDayStarted -= OnDay;
                GameManager.Instance.OnNightStarted -= OnNight;
            }
            
            base.OnDestroy();
        }

        private void Update()
        {
            if (isDay && DaytimeManager.HasInstance)
            {
                var currentTime = DaytimeManager.Instance.CurrentTime;
                var minTime = DaytimeManager.DayStartTime;
                var maxTime = DaytimeManager.DayEndTime;

                var dayProgress =  (currentTime - minTime) / (maxTime - minTime);

                var dayPercent = dayEveningCurve.Evaluate(dayProgress);
                
                dayVolume.weight = dayPercent;
                eveningVolume.weight = 1 - dayPercent;
            }
        }

        private void OnDay(int dayNumber)
        {
            isDay = true;
            dayVolume.weight = 0f;
            eveningVolume.weight = 1f;
            nightVolume.weight = 0f;
        }

        private void OnNight(int nightNumber)
        {
            isDay = false;
            dayVolume.weight = 0f;
            eveningVolume.weight = 0f;
            nightVolume.weight = 1f;
        }
    }
}