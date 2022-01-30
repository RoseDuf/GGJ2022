using UnityEngine;

namespace Game
{
    public class DayNightTransitionCanvasHider : DayNightSensibleMonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        protected override void OnDay(int dayNumber)
        {
            // Do nothing
        }

        protected override void OnNight(int nightNumber)
        {
            // Do Nothing
        }

        protected override void OnDayNightTransitionStarted()
        {
            if (canvasGroup)
                canvasGroup.alpha = 0;
        }
        
        protected override void OnDayNightTransitionFinished()
        {
            if (canvasGroup)
                canvasGroup.alpha = 1;
        }
    }
}