using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //------Singleton------
    private static UIManager _instance;

    public static UIManager Instance
    {
        get { return _instance; }
    }
    //--------------------

    private const int TIME_N = 2;
    [HideInInspector]
    public TextMeshProUGUI[] TimeUI = new TextMeshProUGUI[TIME_N];
    
    
    public RectTransform DayNightCircleRectTransform;
    public RectTransform IndicatorDayNightRectTransform;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }
    
    void OnValidate()
    {
        if (TimeUI.Length != TIME_N)
        {
            Debug.LogWarning("Don't change the number of possible object this object");
            Array.Resize(ref TimeUI, TIME_N);
        }
    }
    
}