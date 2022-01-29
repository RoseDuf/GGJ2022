using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private GameObject itemSlot;

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach(Food food in inventory.Food)
        {
            UIFoodSlot itemSlotData = Instantiate(itemSlot, transform).GetComponent<UIFoodSlot>();
            itemSlotData.SetSlotSprite(food.FoodData.Image);
        }
    }
}
