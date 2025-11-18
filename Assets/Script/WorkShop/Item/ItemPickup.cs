using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    [Header("Item Info")]
    [Tooltip("Id ของไอเท็มให้ตรงกับที่ตั้งใน GameManager.itemDefinitions")]
    public string definitionId;

    [Min(1)]
    public int Amount = 1;

    private ItemDefinition definition;
    private Collider _col;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
    }

    private void Start()
    {
        // ✅ Lookup ที่ Start เพื่อกันลำดับผิดพลาด
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("[ItemPickup] GameManager.Instance is null. Ensure a GameManager exists in the first scene.");
            return;
        }

        if (!string.IsNullOrEmpty(definitionId))
        {
            definition = gm.FindById(definitionId);
            if (definition == null)
            {
                Debug.LogWarning($"[ItemPickup] Definition for Id '{definitionId}' not found in GameManager.");
            }
        }
        else
        {
            Debug.LogWarning("[ItemPickup] definitionId is empty.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (definition == null) return;
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent<Player>(out var player)) return;

        _col.enabled = false; // กันชนซ้ำ

        int added = player.Inventory.Add(definition, Amount);

        if (added >= Amount)
        {
            Destroy(gameObject);
        }
        else
        {
            Amount -= added;
            _col.enabled = true;
            // (optional) แสดงจำนวนคงเหลือบนโลกด้วย floating text ฯลฯ
        }
    }

#if UNITY_EDITOR
    // ทำ quality-of-life: เตือนตั้งค่าใน Editor
    private void OnValidate()
    {
        if (Amount < 1) Amount = 1;
    }
#endif
}