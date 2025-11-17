using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [Header("UI Roots")]
    [SerializeField] private GameObject panelRoot;   // GameObject ที่มี Canvas/Panel ของ Inventory เช่น Canvas -> PanelInventory
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;

    private readonly List<GameObject> slots = new List<GameObject>();
    private bool isOpen = false;

    private void Awake()
    {
        // ให้แน่ใจว่า UI ปิดตอนเริ่มเกม
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    /// เรียกเมื่อต้องการ toggle inventory และ reload รายการภายใน
    public void UpdateInventoryUI(Inventory inventory)
    {
        // Toggle เปิด/ปิด
        isOpen = !isOpen;

        if (isOpen)
        {
            Open(inventory);
        }
        else
        {
            Close();
        }
    }

    private void Open(Inventory inventory)
    {
        if (panelRoot != null && !panelRoot.activeSelf)
            panelRoot.SetActive(true);

        RebuildSlots(inventory);
    }

    private void Close()
    {
        if (panelRoot != null && panelRoot.activeSelf)
            panelRoot.SetActive(false);
        ClearSlots();
    }

    private void RebuildSlots(Inventory inventory)
    {
        ClearSlots();

        if (inventory == null) return;

        foreach (var itemData in inventory.GetAllItems())
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            slots.Add(newSlot);

            // ตั้งชื่อแสดงไปslotPrefabs text component
            var text = newSlot.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = itemData.Name;
            }
        }
    }

    private void ClearSlots()
    {
        // ลบทั้งจากลิสต์และตัวลูกจริง ๆ ใต้ slotParent ให้หมด
        foreach (var s in slots)
        {
            if (s != null) Destroy(s);
        }
        slots.Clear();

        // กัน null ถ้ามีลูกที่สร้างไว้แต่ไม่ได้ถูกเก็บใน slots
        for (int i = slotParent.childCount - 1; i >= 0; i--)
        {
            Destroy(slotParent.GetChild(i).gameObject);
        }
    }
}
