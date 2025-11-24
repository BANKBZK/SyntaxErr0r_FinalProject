
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image icon;
    [SerializeField] private Button useButton;
    [SerializeField] private GameObject selectionHighlight; // optional

    private ItemStack _stack;
    public System.Action<ItemStack> OnUse;
    public System.Action<SlotView> OnSelected;

    // UIInventory จะส่งตัวตรวจสอบเข้ามา เพื่อบอกว่า "usable" หรือไม่
    private System.Func<ItemStack, bool> _isUsablePredicate;

    private void Awake()
    {
        if (useButton) useButton.gameObject.SetActive(false);
        if (selectionHighlight) selectionHighlight.SetActive(false);
    }

    public void Set(ItemStack stack, System.Func<ItemStack, bool> isUsablePredicate = null)
    {
        _stack = stack;
        _isUsablePredicate = isUsablePredicate;

        if (stack.Def == null)
        {
            nameText.text = "-";
            amountText.text = "";
            icon.enabled = false;
            if (useButton) useButton.interactable = false;
            SetSelected(false);
            return;
        }

        nameText.text = stack.Def.DisplayName;
        amountText.text = stack.Amount > 1 ? stack.Amount.ToString() : "";
        icon.sprite = stack.Def.Icon;
        icon.enabled = icon.sprite != null;

        // ตั้งค่า interactable ตาม usable (แต่ซ่อนไว้ก่อนจนกว่าจะ select)
        bool usable = IsUsable();
        if (useButton) useButton.interactable = usable && stack.Amount > 0;
        if (useButton) useButton.gameObject.SetActive(false);
    }

    private bool IsUsable()
    {
        if (_stack.Def == null) return false;
        if (_isUsablePredicate != null) return _isUsablePredicate(_stack);
        // fallback: ใช้กฎจาก definition
        return _stack.Def.Type == ItemType.Heal && _stack.Def.HealAmount > 0;
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight) selectionHighlight.SetActive(selected);

        // โชว์ Use เฉพาะ selected + usable + มีจำนวน
        bool showUse = selected && IsUsable() && _stack.Amount > 0;
        if (useButton) useButton.gameObject.SetActive(showUse);
    }

    public void OnPointerClick(PointerEventData e) => OnSelected?.Invoke(this);
    public void ClickUse() => OnUse?.Invoke(_stack);
}
