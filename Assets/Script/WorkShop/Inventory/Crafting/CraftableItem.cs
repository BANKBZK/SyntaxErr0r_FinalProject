using UnityEngine;

public class CraftableItem : MonoBehaviour, ICraftable
{
    public string craftItemId; // ผูกกับ ItemDefinition Id ของผลลัพธ์

    public void OnCrafted(Player player)
    {
        // ทำอะไรสักอย่าง เช่น แสดงเอฟเฟกต์/ปลดล็อคความสามารถ
        Debug.Log($"Crafted item: {craftItemId}");
    }
}
