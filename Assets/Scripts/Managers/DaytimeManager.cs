using System;
using Game;
using UnityEngine;

public class DaytimeManager : MonoBehaviour
{
    [SerializeField] private DayNightCycle dayNightCycle;

    [Range(0.0f, 1.0f)] public static float DayStartTime = 0.25f;
    [Range(0.0f, 1.0f)] public static float DayEndTime = 0.75f;

    public enum TimeOfDay
    {
        Day,
        Evening,
        Night
    }

    public event Action<TimeOfDay> OnTimeOfDayChanged;

    public TimeOfDay CurrentTimeOfDay;

    [Range(0.0f, 1.0f)] private float _currentTime;

    [HideInInspector]
    public float CurrentTime
    {
        get { return _currentTime; }
        private set { _currentTime = value; }
    }

    private TimeOfDay lastTimeOfDay;

    public void Stop() => dayNightCycle.Stop();
    public void Resume() => dayNightCycle.Resume();

    public static DaytimeManager Instance { get; private set; }
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
    }


    private void Update()
    {
        UpdateTimeOfDay();
    }

    private void UpdateTimeOfDay()
    {
        _currentTime = dayNightCycle.time;
        lastTimeOfDay = CurrentTimeOfDay;

        if (_currentTime > DayStartTime && _currentTime < DayEndTime)
            CurrentTimeOfDay = TimeOfDay.Day;
        else
            CurrentTimeOfDay = TimeOfDay.Night;

        if (CurrentTimeOfDay != lastTimeOfDay)
        {
            OnTimeOfDayChanged?.Invoke(CurrentTimeOfDay);
            Debug.Log("Time of Day: " + CurrentTimeOfDay);
        }
    }
    
}