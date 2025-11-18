using UnityEngine;

[System.Serializable]
public struct ItemStack
{
    public ItemDefinition Def;
    public int Amount;

    public bool IsFull => Amount >= (Def?.MaxStack ?? 0);
    public int Add(int qty)
    {
        int canAdd = Mathf.Min(qty, Def.MaxStack - Amount);
        Amount += canAdd;
        return qty - canAdd; // เหลือที่ยังเติมไม่ลง
    }
}
