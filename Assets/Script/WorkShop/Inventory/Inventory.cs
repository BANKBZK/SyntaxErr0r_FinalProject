
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory
{
    [SerializeField] private List<ItemData> _items = new List<ItemData>();
    public int Capacity { get; private set; }
    ItemData itemData;

    public Inventory(int capacity)
    {
        Capacity = capacity;
    }

    public bool AddItem(Item item)
    {
        if (_items.Count >= Capacity)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        _items.Add(new ItemData(item.Name)); // เก็บข้อมูลชื่อใน Itemdata
        Debug.Log($"Added {item.Name} to inventory");
        return true;
    }

    public bool RemoveItem(ItemData itemData)
    {
        return _items.Remove(itemData);
    }

    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(_items);
    }

    public void ShowInventory()
    {
        Debug.Log("Inventory Items:");
        foreach (var item in _items)
        {
            Debug.Log(item.Name);
        }
    }

    public int ItemCount => _items.Count;
}
