
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CraftingTable : Stuff, IInteractable
{
    [SerializeField] private CraftingManager craftingManager; // ผูกจาก Scene
    [SerializeField] private UICrafting uiCrafting;            // ผูก Panel UI

    private Collider _col;

    bool isOpen;

    public bool isInteractable { get => isLock; set => isLock = value; }

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
    }

    public void Interact(Player player)
    {
        isOpen = !isOpen;
        if (craftingManager == null || uiCrafting == null || player == null)
        {
            Debug.LogWarning("[CraftingTable] Missing references.");
            return;
        }

        if (isOpen)
        {
            // เปิด UI Crafting และส่ง Inventory + Player เข้าไป
            uiCrafting.Open(craftingManager, player.Inventory, player);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        // แสดง Hint "กด E เพื่อเปิดโต๊ะคราฟ" ตามระบบ UI ของคุณ
    }
}
