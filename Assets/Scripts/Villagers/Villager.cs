using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public UIArrow UIArrow { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        UIArrow = GetComponentInChildren<UIArrow>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
