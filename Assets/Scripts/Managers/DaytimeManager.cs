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

    private void Update()
    {
        float currentTime = dayNightCycle.time;

        if (currentTime < 0.25)
        {
            CurrentTimeOfDay = TimeOfDay.Day;
        }
        else if (currentTime < 0.55)
        {
            CurrentTimeOfDay = TimeOfDay.Evening;
        }
        else if (currentTime < 0.7)
        {
            CurrentTimeOfDay = TimeOfDay.Night;
        }
    }
}