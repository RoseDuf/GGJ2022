using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Villager", menuName = "ScriptableObjects/Villager")]
public class VillagerScriptableObject : ScriptableObject
{
    public FoodData.FoodType TypeOfFood;
    public Material Material;
    public float DistanceBetweenHats;
}
