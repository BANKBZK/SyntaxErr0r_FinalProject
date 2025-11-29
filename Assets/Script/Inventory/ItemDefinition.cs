
using UnityEngine;

public enum ItemType
{
    Generic,    // ของทั่วไป ใช้ไม่ได้ (เช่น Wood/Stone)
    Heal,       // ยาฟื้น HP
    // เพิ่มประเภทอื่น ๆ ตามต้องการ เช่น Buff, Mana, Scroll, KeyItem
}

[System.Serializable]
public class ItemDefinition
{
    [Header("Identity")]
    public string Id;                // คีย์เอกลักษณ์ เช่น "potionheal"
    public string DisplayName;       // ชื่อใน UI

    [Header("Visual")]
    public Sprite Icon;              // ไอคอน
    public int MaxStack = 99;        // จำนวนสูงสุดต่อกอง

    [Header("Usage")]
    public ItemType Type = ItemType.Generic;

    [Tooltip("จำนวน HP ที่ฟื้นเมื่อใช้ไอเท็มประเภท Heal")]
    public int HealAmount = 0;

    // (ทางเลือก) เพิ่มฟิลด์อื่นสำหรับประเภทอื่นในอนาคต เช่น ManaAmount, BuffId, Duration เป็นต้น
}
