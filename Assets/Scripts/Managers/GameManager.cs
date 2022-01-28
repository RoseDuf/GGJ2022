using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class GameManager : Singleton<GameManager>
    {
        public event Action<int> OnDayStarted;
        public event Action<int> OnNightStarted;
        public event Action OnDayNightTransitionStarted;
        public event Action OnDayNightTransitionFinished;

        public event Action OnPlayerDied;

        private int currentDay = 0;

        public int CurrentDay => currentDay;
        public DaytimeManager.TimeOfDay CurrentTimeOfDay => DaytimeManager.HasInstance ? DaytimeManager.Instance.CurrentTimeOfDay : DaytimeManager.TimeOfDay.Day;
        public bool IsStartOfGame => currentDay <= 1 && CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day;
        
        private void Start()
        {
            SoundSystem.Instance.PlayDayMusic(); // TODO Move that so we can play night music on swap
            
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

            UIManager.Instance.ShowHealthBar(timeOfDay == DaytimeManager.TimeOfDay.Night);
            
        }

        private IEnumerator TimeOfDayTransitionRoutine(DaytimeManager.TimeOfDay timeOfDay)
        {
            if (timeOfDay == DaytimeManager.TimeOfDay.Day)
                ++currentDay;
            
            if (!IsStartOfGame)
                DaytimeManager.Instance.Stop();
            
            OnDayNightTransitionStarted?.Invoke();
            if (!IsStartOfGame && UIManager.HasInstance)
            {
                UIManager.Instance.ShowDayNightTransition();
                yield return null;
                yield return new WaitUntil(() => UIManager.Instance.DayNightTransitionIsFinished);
            }

            if (timeOfDay == DaytimeManager.TimeOfDay.Day)
                OnDayStarted?.Invoke(currentDay);
            else if (timeOfDay == DaytimeManager.TimeOfDay.Night)
                OnNightStarted?.Invoke(currentDay);
            
            if (!IsStartOfGame)
                yield return new WaitForSeconds(2f);
            
            if (!IsStartOfGame && UIManager.HasInstance)
            {
                UIManager.Instance.HideDayNightTransition();
                yield return null;
                yield return new WaitUntil(() => UIManager.Instance.DayNightTransitionIsFinished);
            }
            
            OnDayNightTransitionFinished?.Invoke();
            DaytimeManager.Instance.Resume();
        }


        public void PlayerDied()
        {
            OnPlayerDied?.Invoke();
        }
    }
}