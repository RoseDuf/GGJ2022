using System.Collections.Generic;

public class Inventory
{
    private List<Food> _food;
    public List<Food> Food { get { return _food; } }
    
    public Inventory()
    {
        _food = new List<Food>();
    }

    public void AddFood(Food food)
    {
        _food.Add(food);
    }

    public void RemoveFood(Food food)
    {
        _food.Remove(food);
    }

    public void EmptyInventory()
    {
        _food = new List<Food>();
    }
}
