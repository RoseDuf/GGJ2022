using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int _foodCapacity;

    public int FoodCapacity { get { return _foodCapacity; } }

    public List<GameObject> Food;

    // Start is called before the first frame update
    void Start()
    {
        Food = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
