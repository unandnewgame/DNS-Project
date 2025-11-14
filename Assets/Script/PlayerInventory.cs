using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int money;
    private BuildableObject turret;
    
    [System.Serializable]
    public class InventorySlot
    {
        public ResourceItem item;
        public int amount;
    }

    public List<InventorySlot> inventory = new List<InventorySlot>();

    public bool HasResources(ItemCost[] cost)
    {
        foreach (var itemCost in cost)
        {
            if (GetAmount(itemCost.resource) < itemCost.amount)
                return false;
        }
        return true;
    }

    public void ConsumeResources(ItemCost[] cost)
    {
        foreach (var itemCost in cost)
        {
            AddAmount(itemCost.resource, -itemCost.amount);
        }
    }

    public void AddResource(ResourceItem resource, int amount)
    {
        AddAmount(resource, amount);
    }

    private int GetAmount(ResourceItem item)
    {
        foreach (var slot in inventory)
        {
            if (slot.item == item) return slot.amount;
        }
        return 0;
    }

    private void AddAmount(ResourceItem item, int amount)
    {
        var slot = inventory.Find(s => s.item == item);
        if (slot != null)
        {
            slot.amount += amount;
            if (slot.amount < 0) slot.amount = 0;
        }
        else if (amount > 0)
        {
            inventory.Add(new InventorySlot { item = item, amount = amount });
        }
    }

    public void SpentMoney(BuildableObject turret)
    {
        money -= turret.turretCost;
    }

    

    public void ClearInventory() => inventory.Clear();
}
