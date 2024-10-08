using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
    public Vector2Int position;
    public int stackLimit = 64;
    public int quantity;
    public ItemClass item;
    public InventorySlot()
    {

    }
    public InventorySlot(InventorySlot slot)
    {
        position = slot.position;
        stackLimit = slot.stackLimit;
        quantity = slot.quantity;
        item = slot.item;
    }
}
