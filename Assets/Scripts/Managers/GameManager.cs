using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public event Action<int> OnDayStarted;
        public event Action<int> OnNightStarted;
        public event Action OnDayNightTransitionStarted;
        public event Action OnDayNightTransitionFinished;

        public event Action OnPlayerDied;

        private int currentDay = 1;

        public int CurrentDay => currentDay;
        public DaytimeManager.TimeOfDay CurrentTimeOfDay => DaytimeManager.HasInstance ? DaytimeManager.Instance.CurrentTimeOfDay : DaytimeManager.TimeOfDay.Day;
        public bool IsStartOfGame => currentDay <= 1 && CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day;


        private UIManager _uiManager;

        public static GameManager Instance { get; private set; }
        public static bool HasInstance { get { return Instance != null; } }

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _uiManager = UIManager.Instance;
        }

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

        protected void OnDestroy()
        {
            if (DaytimeManager.HasInstance)
            {
                DaytimeManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;
            }
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

            if (UIManager.HasInstance)
            {
                UIManager.Instance.ShowHealthBar(timeOfDay == DaytimeManager.TimeOfDay.Night);
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