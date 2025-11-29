[System.Serializable]
public class Quest
{
    public string title;
    public string description;
    public bool isCompleted;
    public bool isActive;

    // เพิ่มส่วนนี้เพื่อระบุไอเท็มที่ต้องใช้
    public ItemDefinition requiredItem;
    public int requiredAmount;

    public Quest(string title, string description, ItemDefinition item, int amount)
    {
        this.title = title;
        this.description = description;
        this.requiredItem = item;
        this.requiredAmount = amount;
        this.isActive = false;
        this.isCompleted = false;
    }
}