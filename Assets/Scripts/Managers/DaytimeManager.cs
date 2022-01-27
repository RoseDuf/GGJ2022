using System;
using Game;
using UnityEngine;

public class DaytimeManager : Singleton<DaytimeManager>
{
    [SerializeField] private DayNightCycle dayNightCycle;
    
    [Range(0.0f, 1.0f)]
    public static float DayStartTime = 0.25f;
    [Range(0.0f, 1.0f)]
    public static float DayEndTime = 0.75f;
    public enum TimeOfDay
    {
        Day,
        Evening,
        Night
    }

    public event Action<TimeOfDay> OnTimeOfDayChanged;

    public TimeOfDay CurrentTimeOfDay;
    private double hour = 0;
    private double minute = 0;

    [Range(0.0f, 1.0f)] private float currentTime;

    private TimeOfDay lastTimeOfDay;

    public void Stop() => dayNightCycle.Stop();
    public void Resume() => dayNightCycle.Resume();

    private void Update()
    {
        UpdateTimeOfDay();
        UpdateIndicatorPosition();
    }

    private void UpdateTimeOfDay()
    {
        currentTime = dayNightCycle.time;
        lastTimeOfDay = CurrentTimeOfDay;
        
        if (currentTime > DayStartTime && currentTime < DayEndTime)
            CurrentTimeOfDay = TimeOfDay.Day;
        else
            CurrentTimeOfDay = TimeOfDay.Night;

        if (CurrentTimeOfDay != lastTimeOfDay)
        {
            OnTimeOfDayChanged?.Invoke(CurrentTimeOfDay);
            Debug.Log("Time of Day: " + CurrentTimeOfDay);
        }
    }

    private void UpdateIndicatorPosition()
    {
        float timeAngle = (currentTime * -360);

        Quaternion quat;
        
        if (timeAngle < -90 && timeAngle >= -270)
        {
            // Day
            UIManager.Instance.DayNightCircleRectTransform.rotation = Quaternion.Euler(0,0,0);
            quat = Quaternion.Euler(0,0,timeAngle);
        }
        else
        {
            // Night
            UIManager.Instance.DayNightCircleRectTransform.rotation = Quaternion.Euler(0, 0, -180);
            quat = Quaternion.Euler(0,0,timeAngle - 180);
        }
        
        UIManager.Instance.IndicatorDayNightRectTransform.rotation = quat;
    }

    private void UpdateCycleCircle()
    {
        
    }

    private void UpdateHourMinute()
    {
        double time24 = currentTime * 24;
        hour = Math.Truncate(time24);
        minute = (time24 - hour) * 60;
    }
}