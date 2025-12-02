using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventory : MonoBehaviour
{
    [Header("UI Roots")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;

    private readonly List<GameObject> pool = new();
    private Inventory inv;
    private Player player;
    private SlotView currentSelected;

    public void Bind(Inventory inventory, Player playerRef = null)
    {
        if (inv != null) inv.Changed -= Refresh;
        inv = inventory;
        if (inv != null) inv.Changed += Refresh;

        player = playerRef ?? FindAnyPlayer();

        if (panelRoot != null) panelRoot.SetActive(false);

        // ✅ เริ่มเกม: ซ่อนเมาส์ทันที (เพราะ Inventory ปิดอยู่)
        UpdateCursorState(false);

        Refresh();
    }

    public void Toggle()
    {
        bool show = !panelRoot.activeSelf;
        panelRoot.SetActive(show);

        // ✅ เรียกใช้ฟังก์ชันจัดการเมาส์ตามสถานะ show
        UpdateCursorState(show);

        if (!show) SetSelected(null);
        if (show) Refresh();
    }

    // ---------------- Cursor Logic ----------------
    // ฟังก์ชันช่วยจัดการเมาส์ (แยกออกมาให้ดูง่าย)
    private void UpdateCursorState(bool isOpen)
    {
        if (isOpen)
        {
            // ถ้าเปิดกระเป๋า: โชว์เมาส์ + ปลดล็อคให้ขยับไปกดของได้
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // ถ้าปิดกระเป๋า: ซ่อนเมาส์ + ล็อคไว้กลางจอ (สำหรับมุมมอง FPS/TPS)
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    // ---------------------------------------------

    private void OnDestroy()
    {
        if (inv != null) inv.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (inv == null) return;

        EnsurePool(inv.Slots.Count);

        for (int i = 0; i < pool.Count; i++)
            pool[i].SetActive(i < inv.Slots.Count);

        for (int i = 0; i < inv.Slots.Count; i++)
        {
            var view = pool[i].GetComponent<SlotView>();
            var stack = inv.Slots[i];

            view.Set(stack, IsUsableItem);

            view.OnSelected = v => SetSelected(v);
            view.OnUse = s => TryUseItem(s);
        }

        SetSelected(null);
    }

    private void EnsurePool(int needed)
    {
        while (pool.Count < needed)
        {
            var go = Instantiate(slotPrefab, slotParent);
            pool.Add(go);
        }
    }

    private void SetSelected(SlotView v)
    {
        if (currentSelected != null) currentSelected.SetSelected(false);
        currentSelected = v;
        if (currentSelected != null) currentSelected.SetSelected(true);
    }

    // ---------------- Usability & Use Logic ----------------

    private bool IsUsableItem(ItemStack stack)
    {
        if (stack.Def == null || stack.Amount <= 0) return false;

        switch (stack.Def.Type)
        {
            case ItemType.Heal:
                return stack.Def.HealAmount > 0;
            default:
                return false;
        }
    }

    private void TryUseItem(ItemStack stack)
    {
        if (!IsUsableItem(stack)) return;

        var def = stack.Def;
        if (!inv.Remove(def, 1)) return;

        switch (def.Type)
        {
            case ItemType.Heal:
                player?.Heal(def.HealAmount);
                break;
        }

        SetSelected(null);
    }

    private Player FindAnyPlayer()
    {
#if UNITY_2023_1_OR_NEWER
        return Object.FindAnyObjectByType<Player>();
#else
        return FindObjectOfType<Player>();
#endif
    }

    private void Update()
    {
        if (!panelRoot.activeSelf) return;
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            SetSelected(null);
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}