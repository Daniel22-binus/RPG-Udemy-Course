using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UI_ItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemText;

    public InventoryItem item;

    public void UpdateSlot(InventoryItem newItem)
    {
        item = newItem;
        itemImage.color = Color.white;
        itemText.faceColor = Color.white;

        itemImage.sprite = item.data.icon;
        if (item.StackSize > 1)
        {
            itemText.text = item.StackSize.ToString();
        }
        else
        {
            itemText.text = "";
        }
    }
}
