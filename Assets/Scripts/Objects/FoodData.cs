using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Food Data", menuName = "ScriptableObjects/FoodData")]
public class FoodData : ScriptableObject
{
    public enum FoodType
    {
        Apple,
        Fish,
        Bread,
        Cheese
    }

    public FoodType TypeOfFood;
    public Material Material;
    public Mesh Mesh;
}
