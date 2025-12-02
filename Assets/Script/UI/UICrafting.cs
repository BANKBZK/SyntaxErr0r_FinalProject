using System.Collections.Generic;
using UnityEngine;

public class UICrafting : MonoBehaviour
{
    [Header("UI Roots")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform recipeListParent;
    [SerializeField] private GameObject recipeEntryPrefab;

    private CraftingManager manager;
    private Inventory inv;
    private Player player;

    private readonly List<GameObject> entries = new();

    /// <summary>
    /// เปิด UI Crafting และสร้างรายการสูตรทั้งหมด
    /// </summary>
    public void Open(CraftingManager craftingManager, Inventory inventory, Player playerRef)
    {
        manager = craftingManager;
        inv = inventory;
        player = playerRef;

        panelRoot.SetActive(true);

        // ✅ เปิดหน้าต่าง -> โชว์เมาส์
        UpdateCursorState(true);

        // (Optional) เล่นเสียงเปิด
        if (SoundManager.instance != null) SoundManager.instance.PlaySFX("OpenMenu");

        RebuildList();
    }

    /// <summary>
    /// ปิด UI Crafting
    /// </summary>
    public void Close()
    {
        panelRoot.SetActive(false);
        UpdateCursorState(false);
        ClearList();
    }

    // ---------------- Cursor Logic ----------------
    private void UpdateCursorState(bool isOpen)
    {
        if (isOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // ขยับเมาส์ได้อิสระ
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; // ล็อคเป้ากลางจอ
        }
    }
    // ---------------------------------------------

    private void ClearList()
    {
        foreach (var go in entries)
            if (go) Destroy(go);
        entries.Clear();
    }

    private void RebuildList()
    {
        ClearList();
        if (manager == null || inv == null) return;

        // แสดงสูตรทั้งหมด แต่ craftButton จะ disable ถ้าไม่ครบ
        foreach (var recipe in manager.recipes)
        {
            if (recipe == null) continue;

            var go = Instantiate(recipeEntryPrefab, recipeListParent);
            entries.Add(go);

            var view = go.GetComponent<RecipeEntryView>();
            view.Set(recipe, inv);

            // เมื่อกด craft บนรายการนี้
            view.onCraftClicked = () =>
            {
                bool ok = manager.TryCraft(recipe, inv, player);
                if (ok)
                {
                    // รีเฟรช UI เพื่ออัปเดตจำนวนของ/สถานะปุ่ม
                    RebuildList();
                }
            };
        }
    }
}