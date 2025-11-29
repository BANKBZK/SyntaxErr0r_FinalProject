using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : Stuff, IInteractable, IQuestGiver
{
    // สร้างตัวเลือกโหมดการทำงาน
    public enum NPCType
    {
        GiveItem,       // แบบที่ 1: ให้ของรางวัลกับผู้เล่น
        UnlockObject    // แบบที่ 2: ปลดล็อคประตู/กล่อง
    }

    [Header("NPC Settings")]
    public NPCType npcType = NPCType.GiveItem;

    [Header("Quest Requirement (Player ต้องเอาอะไรมาแลก)")]
    [Tooltip("ID ของไอเท็มที่ NPC ต้องการ (ต้องตรงกับ InventoryManagement)")]
    public string requiredItemId;
    public int requiredAmount = 1;

    [Header("Reward Type 1: Give Item")]
    [Tooltip("ID ของรางวัลที่จะให้ผู้เล่น (ถ้าเลือกโหมด GiveItem)")]
    public string rewardItemId;
    public int rewardAmount = 1;

    [Header("Reward Type 2: Unlock Object")]
    [Tooltip("ลากประตู หรือ Stuff ที่ต้องการปลดล็อคมาใส่ตรงนี้")]
    public Stuff objectToUnlock;

    [Header("Dialogs")]
    public string questDesc = "I need a key.";
    public string completeDesc = "Thank you!";
    public TMP_Text WordTextUI;

    // Internal Variables
    private Quest currentQuest;
    private ItemDefinition _requiredItemDef;
    private ItemDefinition _rewardItemDef; // สำหรับโหมดให้ของ
    public bool canTalk = true;
    public bool isInteractable { get => canTalk; set => canTalk = value; }

    public override void SetUP()
    {
        base.SetUP();
        if (WordTextUI != null) WordTextUI.gameObject.SetActive(false);
        SetupQuestData();
    }

    private void SetupQuestData()
    {
        var im = InventoryManagement.Instance;
        if (im == null) return;

        // 1. หาข้อมูลของที่ NPC อยากได้
        if (!string.IsNullOrEmpty(requiredItemId))
            _requiredItemDef = im.FindById(requiredItemId);

        // 2. หาข้อมูลของรางวัล (ถ้าเป็นโหมดให้ของ)
        if (npcType == NPCType.GiveItem && !string.IsNullOrEmpty(rewardItemId))
            _rewardItemDef = im.FindById(rewardItemId);

        // สร้าง Object Quest จำลองขึ้นมา
        if (_requiredItemDef != null)
        {
            currentQuest = new Quest("Quest", questDesc, _requiredItemDef, requiredAmount);
        }
        else
        {
            Debug.LogError($"[NPC] หาไอเท็ม ID '{requiredItemId}' ไม่เจอ!");
            canTalk = false; // ปิดการคุยถ้าข้อมูลผิด
        }
    }

    public void Interact(Player player)
    {
        if (!canTalk || _requiredItemDef == null) return;

        // ถ้าเควสจบไปแล้ว
        if (currentQuest.isCompleted)
        {
            ShowDialog(completeDesc);
            return;
        }

        // ถ้ารับเควสแล้ว -> เช็คของเพื่อส่งเควส
        if (currentQuest.isActive)
        {
            CheckAndCompleteQuest(player);
        }
        else // ยังไม่รับเควส -> รับเควส
        {
            StartQuest(currentQuest);
            ShowDialog($"{currentQuest.description}\n(Need: {requiredAmount} {_requiredItemDef.DisplayName})");
        }
    }

    private void CheckAndCompleteQuest(Player player)
    {
        Inventory playerInv = player.Inventory;
        if (playerInv == null) return;

        int count = playerInv.CountOf(_requiredItemDef);

        if (count >= requiredAmount)
        {
            // 1. ลบของจากตัวผู้เล่น (ของที่ NPC อยากได้)
            playerInv.Remove(_requiredItemDef, requiredAmount);

            // 2. ให้รางวัลตามประเภท NPC
            GiveReward(player);

            // 3. จบเควส
            CompleteQuest(currentQuest);
        }
        else
        {
            ShowDialog($"I still need {_requiredItemDef.DisplayName}.\nYou have {count}/{requiredAmount}.");
        }
    }

    private void GiveReward(Player player)
    {
        if (npcType == NPCType.GiveItem)
        {
            // แบบที่ 1: ให้ของ
            if (_rewardItemDef != null)
            {
                player.Inventory.Add(_rewardItemDef, rewardAmount);
                ShowDialog($"Here is your {_rewardItemDef.DisplayName}!");
            }
        }
        else if (npcType == NPCType.UnlockObject)
        {
            // แบบที่ 2: ปลดล็อค Stuff (ประตู)
            if (objectToUnlock != null)
            {
                objectToUnlock.isUnlock = true; // สั่งปลดล็อคตรงนี้!
                ShowDialog("The door is unlocked now!");

                // Optional: ถ้าเป็นประตู อยากให้สั่งเปิดเลยไหม? 
                // ถ้าอยากให้เปิดเลย ให้ Cast เป็น Door แล้วสั่ง Interact ก็ได้
                // แต่ปกติแค่ปลดล็อคให้ผู้เล่นไปกดเปิดเองจะดีกว่า
            }
            else
            {
                Debug.LogWarning("[NPC] ลืมลาก objectToUnlock มาใส่ใน Inspector!");
            }
        }
    }

    // Override Update เพื่อแก้บั๊ก UI ซ้อนกัน (จากโค้ดชุดก่อน)
    public override void Update()
    {
        bool isTalking = WordTextUI.gameObject.activeSelf;
        if (isTalking)
        {
            if (interactionTextUI != null && interactionTextUI.gameObject.activeSelf)
                interactionTextUI.gameObject.SetActive(false);
            return;
        }
        base.Update();
    }

    public void StartQuest(Quest quest) { quest.isActive = true; }
    public void CompleteQuest(Quest quest) { quest.isActive = false; quest.isCompleted = true; StartCoroutine(CloseDialogAfterDelay(3f)); }
    private void ShowDialog(string msg) { WordTextUI.text = msg; WordTextUI.gameObject.SetActive(true); }
    IEnumerator CloseDialogAfterDelay(float d) { yield return new WaitForSeconds(d); WordTextUI.gameObject.SetActive(false); }
    public bool CanGiveQuest() => !currentQuest.isActive;
}