using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DaytimeManager : MonoBehaviour
{
    private static DaytimeManager _instance;
    
    public static DaytimeManager Instance { get { return _instance; } }

    [SerializeField]
    private DayNightCycle dayNightCycle;
    
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

        if (dayNightCycle != null)
        {
            dayNightCycle.nightThreshold += UpdateCurrentTimeOfDay;
        }
    }

    void UpdateCurrentTimeOfDay(object sender, DayNightCycle.ThresholdReachedEventArgs e)
    {
        CurrentTimeOfDay = e.Period;
        Debug.Log("Time of day: " + CurrentTimeOfDay.ToString());
    }
}
