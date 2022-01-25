using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class GameManager : Singleton<GameManager>
    {
        public event Action<int> OnDayStarted;
        public event Action<int> OnNightStarted;
        public event Action OnDayNightTransitionFinished;

        private int currentDay = 1;

        public int CurrentDay => currentDay;
        public DaytimeManager.TimeOfDay CurrentTimeOfDay => DaytimeManager.HasInstance ? DaytimeManager.Instance.CurrentTimeOfDay : DaytimeManager.TimeOfDay.Day;

        private void Start()
        {
            if (!DaytimeManager.HasInstance)
            {
                Debug.Log($"{nameof(GameManager)} could not find any {nameof(DaytimeManager)}. The game cannot be started.");
                return;
            }

            DaytimeManager.Instance.OnTimeOfDayChanged += OnTimeOfDayChanged;
        }

        protected override void OnDestroy()
        {
            if (DaytimeManager.HasInstance)
            {
                DaytimeManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;
            }
            
            base.OnDestroy();
        }

        private void OnTimeOfDayChanged(DaytimeManager.TimeOfDay timeOfDay)
        {
            if (timeOfDay == DaytimeManager.TimeOfDay.Day || timeOfDay == DaytimeManager.TimeOfDay.Night)
            {
                StartCoroutine(TimeOfDayTransitionRoutine(timeOfDay));
            }
        }

        private IEnumerator TimeOfDayTransitionRoutine(DaytimeManager.TimeOfDay timeOfDay)
        {
            DaytimeManager.Instance.Stop();
            // TODO Black Screen hiding things
            if (timeOfDay == DaytimeManager.TimeOfDay.Day)
            {
                ++currentDay;
                OnDayStarted?.Invoke(currentDay);
            }
            else if (timeOfDay == DaytimeManager.TimeOfDay.Night)
            {
                OnNightStarted?.Invoke(currentDay);
            }
            
            yield return new WaitForSeconds(2f); // TODO Wait until everything has changed
            // TODO Black screen disappears
            OnDayNightTransitionFinished?.Invoke();
        }
    }
}