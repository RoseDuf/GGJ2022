using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new Food Data", menuName = "ScriptableObjects/FoodData")]
public class FoodData : ScriptableObject
{
    public enum FoodType
    {
        Apple,
        Fish,
        Cake,
        Cheese
    }

    public FoodType TypeOfFood;
    public Material Material;
    public Mesh Mesh;
    public Sprite Image;
    public Vector3 Rotation;
    public float Scale;
}
