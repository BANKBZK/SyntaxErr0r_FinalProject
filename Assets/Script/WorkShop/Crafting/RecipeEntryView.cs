using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeEntryView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image resultIcon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text ingredientsText;
    [SerializeField] private Button craftButton;

    private CraftingRecipe _recipe;
    private Inventory _inv;

    public System.Action onCraftClicked;

    public void Set(CraftingRecipe recipe, Inventory inv)
    {
        _recipe = recipe;
        _inv = inv;

        titleText.text = recipe.DisplayName;

        // แสดงรายการวัตถุดิบ: ชื่อ + ต้องใช้ + ที่มี
        var sb = new StringBuilder();
        foreach (var ing in recipe.Ingredients)
        {
            var def = InventoryManagement.Instance.FindById(ing.ItemId);
            int have = (def != null) ? _inv.CountOf(def) : 0;
            string name = (def != null) ? def.DisplayName : ing.ItemId;

            sb.AppendLine($"{name} x{ing.Amount} (have {have})");
        }
        ingredientsText.text = sb.ToString();


        // ไอคอนผลลัพธ์ (ถ้ามี)
        var resDef = InventoryManagement.Instance.FindById(recipe.ResultItemId);
        if (resDef != null)
        {
            resultIcon.sprite = resDef.Icon;
            resultIcon.enabled = resDef.Icon != null;
        }
        else
        {
            resultIcon.enabled = false;
        }

        // ปุ่ม craft สามารถกดได้เฉพาะเมื่อมีของครบ
        craftButton.interactable = recipe.CanCraft(_inv);
    }

    public void ClickCraft()
    {
        onCraftClicked?.Invoke();
    }
}
