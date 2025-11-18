using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlotView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text amountText;
    [SerializeField] Image icon;

    public void Set(ItemStack stack)
    {
        nameText.text = stack.Def.DisplayName;
        amountText.text = stack.Amount > 1 ? stack.Amount.ToString() : "";
        icon.sprite = stack.Def.Icon;
        icon.enabled = icon.sprite != null;
    }
}
