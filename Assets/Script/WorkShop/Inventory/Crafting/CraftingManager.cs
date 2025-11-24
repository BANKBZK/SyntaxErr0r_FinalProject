
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [Header("All Recipes")]
    public List<CraftingRecipe> recipes = new();

    /// <summary>
    /// หา recipe ตาม Id
    /// </summary>
    public CraftingRecipe FindRecipe(string recipeId)
    {
        return recipes.Find(r => r != null && r.RecipeId == recipeId);
    }

    /// <summary>
    /// คราฟสูตรตามที่ส่งมา (เช็คของ → ตัดของ → ให้ผลลัพธ์)
    /// </summary>
    public bool TryCraft(CraftingRecipe recipe, Inventory inv, Player player)
    {
        if (recipe == null || inv == null)
        {
            Debug.LogWarning("[CraftingManager] Recipe or Inventory is null.");
            return false;
        }

        // 1) เช็ควัตถุดิบครบไหม
        if (!recipe.CanCraft(inv))
        {
            Debug.Log("Not enough materials.");
            return false;
        }

        // 2) ตัดวัตถุดิบ
        if (!recipe.ConsumeIngredients(inv))
        {
            Debug.Log("Failed to consume ingredients.");
            return false;
        }

        // 3) ให้ผลลัพธ์เข้าคลัง
        if (!recipe.GiveResult(inv))
        {
            Debug.Log("Failed to give result item (inventory full?).");
            return false;
        }

        // 4) Hook ICraftable (optional): ถ้ามีตัวลงโลก/ทำอะไรพิเศษตอนคราฟ
        // var itemGO = Instantiate(...); itemGO.GetComponent<ICraftable>()?.OnCrafted(player);

        Debug.Log($"Crafted: {recipe.DisplayName}");
        return true;
    }

    /// <summary>
    /// รายการสูตรทั้งหมดที่ "คราฟได้ตอนนี้"
    /// </summary>
    public List<CraftingRecipe> GetCraftableNow(Inventory inv)
    {
        var list = new List<CraftingRecipe>();
        foreach (var r in recipes)
        {
            if (r != null && r.CanCraft(inv))
                list.Add(r);
        }
        return list;
    }
}
