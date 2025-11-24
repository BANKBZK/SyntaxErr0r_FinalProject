
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftingRecipe
{
    [Header("Recipe Info")]
    public string RecipeId;          // เช่น "axe_01"
    public string DisplayName;       // เช่น "Stone Axe"

    [Header("Ingredients")]
    public List<CraftingIngredient> Ingredients = new();

    [Header("Result")]
    [Tooltip("Id ของผลลัพธ์ เช่น 'axe'")]
    public string ResultItemId;
    [Min(1)]
    public int ResultAmount = 1;

    /// <summary>
    /// ตรวจว่า Inventory มีวัตถุดิบครบตามสูตรหรือไม่
    /// </summary>
    public bool CanCraft(Inventory inv)
    {
        if (inv == null) return false;

        foreach (var ing in Ingredients)
        {
            var def = InventoryManagement.Instance.FindById(ing.ItemId);
            if (def == null) return false;

            if (inv.CountOf(def) < ing.Amount)
                return false;
        }
        return true;
    }

    /// <summary>
    /// ตัดวัตถุดิบออกจาก Inventory (ให้เรียกหลัง CanCraft == true เท่านั้น)
    /// </summary>
    public bool ConsumeIngredients(Inventory inv)
    {
        if (!CanCraft(inv)) return false;

        foreach (var ing in Ingredients)
        {
            var def = InventoryManagement.Instance.FindById(ing.ItemId);
            if (def == null) return false;

            if (!inv.Remove(def, ing.Amount))
            {
                // ถ้าต้องการระบบ rollback เมื่อบางชิ้นลบไม่ได้ ให้เพิ่ม logic ที่นี่
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// เพิ่มผลลัพธ์เข้าคลัง
    /// </summary>
    public bool GiveResult(Inventory inv)
    {
        var resultDef = InventoryManagement.Instance.FindById(ResultItemId);
        if (resultDef == null) return false;

        int added = inv.Add(resultDef, ResultAmount);
        return added >= ResultAmount;
    }
}
