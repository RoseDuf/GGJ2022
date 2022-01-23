using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DaytimeManager : MonoBehaviour
{
    private static DaytimeManager _instance;

    public static DaytimeManager Instance
    {
        get { return _instance; }
    }

    [SerializeField] private DayNightCycle dayNightCycle;

    public enum TimeOfDay
    {
        Day,
        Evening,
        Night
    }

    public TimeOfDay CurrentTimeOfDay;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private TimeOfDay lastTimeOfDay;
    private void Update()
    {
        float currentTime = dayNightCycle.time;
        lastTimeOfDay = CurrentTimeOfDay;

        if (currentTime >= 0.25 && currentTime < 0.55)
        {
            CurrentTimeOfDay = TimeOfDay.Day;
        }
        else if (currentTime >= 0.55 && currentTime < 0.75) 
        {
            CurrentTimeOfDay = TimeOfDay.Evening;
        }
        else if (currentTime >= 0.6)
        {
            CurrentTimeOfDay = TimeOfDay.Night;
            
        }

        if (CurrentTimeOfDay != lastTimeOfDay)
        {
            Debug.Log("Time of Day: " + CurrentTimeOfDay.ToString());
        }
    }
}