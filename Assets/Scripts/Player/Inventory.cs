using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<Food> Food { get; }
    
    public Inventory()
    {
        Food = new List<Food>();
    }

    public void AddFood(Food food)
    {
        Food.Add(food);
    }

    public void RemoveFood(Food food)
    {
        Food.Remove(food);
    }
}
