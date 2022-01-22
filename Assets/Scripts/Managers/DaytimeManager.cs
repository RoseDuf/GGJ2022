using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaytimeManager : MonoBehaviour
{
    private static DaytimeManager _instance;

    public static DaytimeManager Instance { get { return _instance; } }

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
}
