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
    [Tooltip("0 : HOUR_UI\n1 : MINUTE_UI")]
    public TextMeshProUGUI[] TimeUI = new TextMeshProUGUI[TIME_N];
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        InstantiateUI();
    }

    private void InstantiateUI()
    {
        Instantiate(TimeUI[0]);
        Instantiate(TimeUI[1]);
        //Instantiate(Canva)
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