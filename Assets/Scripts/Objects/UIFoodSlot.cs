using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFoodSlot : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    public void SetSlotSprite(Sprite newSprite)
    {
        _image.sprite = newSprite;
    }
    
}
