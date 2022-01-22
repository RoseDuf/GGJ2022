using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public enum FoodType
    {
        Apple,
        Fish,
        Bread,
        Cheese
    }

    public UIArrow UIArrow { get; set; }

    [SerializeField]
    private FoodType _typeOfFood;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    public int Fatness;

    public FoodType GetFoodType { get { return _typeOfFood; } }


    // Start is called before the first frame update
    void Start()
    {
        UIArrow = GetComponentInChildren<UIArrow>();
        Fatness = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EatFood()
    {
        Fatness += 1;
    }
}
