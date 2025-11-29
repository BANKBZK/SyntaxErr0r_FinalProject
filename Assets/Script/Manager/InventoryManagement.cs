using System.Collections.Generic;
using UnityEngine;

public class InventoryManagement : MonoBehaviour
{
    public static InventoryManagement Instance { get; private set; }

    [Header("Item Database")]
    public List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // ✅ คงอยู่ข้ามซีน
        DontDestroyOnLoad(gameObject);
    }
    public ItemDefinition FindById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        return itemDefinitions.Find(d => d != null && d.Id == id);
    }
}
