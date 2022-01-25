using UnityEngine;

namespace Game
{
    public abstract class DayNightSensibleMonoBehaviour : MonoBehaviour
    {
        protected virtual void Start()
        {
            if (!GameManager.HasInstance)
                Debug.LogWarning($"A {nameof(DayNightSensibleMonoBehaviour)} ({gameObject.name}) is placed in a scene without any {nameof(GameManager)}." +
                                 $"It will not be able to react to time events ({nameof(OnDay)} and {nameof(OnNight)} will never be called).");
        }

        protected virtual void OnEnable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnDayStarted += OnDay;
                GameManager.Instance.OnNightStarted += OnNight;
                GameManager.Instance.OnDayNightTransitionStarted += OnDayNightTransitionStarted;
                GameManager.Instance.OnDayNightTransitionFinished += OnDayNightTransitionFinished;
            }
        }

        protected void OnDisable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnDayStarted -= OnDay;
                GameManager.Instance.OnNightStarted -= OnNight;
                GameManager.Instance.OnDayNightTransitionStarted -= OnDayNightTransitionStarted;
                GameManager.Instance.OnDayNightTransitionFinished -= OnDayNightTransitionFinished;
            }
        }

        protected abstract void OnDay(int dayNumber);
        protected abstract void OnNight(int nightNumber);
        protected virtual void OnDayNightTransitionStarted() {}
        protected virtual void OnDayNightTransitionFinished() {}
    }
}
