using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public List<InventoryItem> inventoryItem;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform itemSlotParent;
    private UI_ItemSlot[] itemSlots;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }
    void Start()
    {
        inventoryItem = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        itemSlots = itemSlotParent.GetComponentsInChildren<UI_ItemSlot>();
    }

    void Update()
    {
        
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < inventoryItem.Count; i++)
        {
            itemSlots[i].UpdateSlot(inventoryItem[i]);
        }
    }

    public void AddItem(ItemData item)
    {
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(item);
            inventoryItem.Add(newItem);
            inventoryDictionary.Add(item, newItem);
        }

        UpdateSlotUI();
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.StackSize <= 1)
            {
                inventoryItem.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
            {
                value.RemoveStack();
            }
            UpdateSlotUI();
        }

    }
}
