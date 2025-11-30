using UnityEngine;

public class ItemPickOnInteract : Stuff, IInteractable
{
    [Header("Item Info")]
    [Tooltip("Id ของไอเท็มให้ตรงกับที่ตั้งใน InventoryManagement.itemDefinitions")]
    public string definitionId;

    [Min(1)]
    public int Amount = 1;

    private ItemDefinition definition;
    public bool isInteractable { get => isUnlock; set => isUnlock = value; }

    private void Start()
    {
        // ✅ Lookup ที่ Start เพื่อกันลำดับผิดพลาด
        var im = InventoryManagement.Instance;
        if (im == null)
        {
            Debug.LogError("[ItemPickup] InventoryManagement.Instance is null. Ensure a InventoryManagement exists in the first scene.");
            return;
        }

        if (!string.IsNullOrEmpty(definitionId))
        {
            definition = im.FindById(definitionId);
            if (definition == null)
            {
                Debug.LogWarning($"[ItemPickup] Definition for Id '{definitionId}' not found in InventoryManagement.");
            }
        }
        else
        {
            Debug.LogWarning("[ItemPickup] definitionId is empty.");
        }
    }
    public void Interact(Player player)
    {
        int added = player.Inventory.Add(definition, Amount);

        if (added >= Amount)
        {
            Destroy(gameObject);
        }
    }

}
