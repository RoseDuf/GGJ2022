using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDatabase : MonoBehaviour
{
    private static FoodDatabase _instance;

    public static FoodDatabase Instance { get { return _instance; } }

    [SerializeField]
    private List<FoodData> _foodData;

    public List<FoodData> FoodData { get { return _foodData; } }

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
