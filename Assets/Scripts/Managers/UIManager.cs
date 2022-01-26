using System;
using Game;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private const int TIME_N = 2;
    private static readonly int Show = Animator.StringToHash("Show");
    private static readonly int Hide = Animator.StringToHash("Hide");

    [SerializeField] private Animator dayNightTransitionAnimator;
    [HideInInspector] public TextMeshProUGUI[] TimeUI = new TextMeshProUGUI[TIME_N];
    
    public RectTransform DayNightCircleRectTransform;
    public RectTransform IndicatorDayNightRectTransform;

    public bool DayNightTransitionIsFinished => !dayNightTransitionAnimator.IsInTransition(0) &&
                                                dayNightTransitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;

    void OnValidate()
    {
        if (TimeUI.Length != TIME_N)
        {
            Debug.LogWarning("Don't change the number of possible object this object");
            Array.Resize(ref TimeUI, TIME_N);
        }
    }

    public void ShowDayNightTransition()
    {
        if (dayNightTransitionAnimator)
        {
            dayNightTransitionAnimator.SetTrigger(Show);
        }
    }

    public void HideDayNightTransition()
    {
        if (dayNightTransitionAnimator)
        {
            dayNightTransitionAnimator.SetTrigger(Hide);
        }
    }
    
}