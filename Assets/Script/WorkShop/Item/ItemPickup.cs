using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    public ItemDefinition Definition;
    public int Amount = 1;

    Collider _col;

    void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent<Player>(out var player)) return;

        _col.enabled = false; // กันชนซ้ำ
        int added = player.Inventory.Add(Definition, Amount);

        if (added >= Amount) Destroy(gameObject);
        else
        {
            Amount -= added;
            _col.enabled = true; // อนุญาตเก็บต่อในรอบหน้า
        }
    }
}
