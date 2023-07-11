using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int StackSize;
    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData;
        StackSize = 1;
    }

    public void AddStack() => StackSize++;
    public void RemoveStack() => StackSize--;
}
