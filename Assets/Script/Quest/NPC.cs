using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : Stuff, IInteractable, IQuestGiver
{
    // สร้างตัวเลือกโหมดการทำงาน
    public enum NPCType
    {
        GiveItem,       // แบบที่ 1: ให้ของรางวัลกับผู้เล่น
        UnlockObject,   // แบบที่ 2: ปลดล็อคประตู/กล่อง
        SetActiveObject // แบบที่ 3: (ใหม่) สั่งเปิด/ปิด GameObject
    }

    [Header("NPC Settings")]
    public NPCType npcType = NPCType.GiveItem;

    [Header("Quest Requirement (Player ต้องเอาอะไรมาแลก)")]
    [Tooltip("ID ของไอเท็มที่ NPC ต้องการ (ต้องตรงกับ InventoryManagement)")]
    public string requiredItemId;
    public int requiredAmount = 1;

    [Header("Reward Type 1: Give Item")]
    [Tooltip("ID ของรางวัลที่จะให้ผู้เล่น")]
    public string rewardItemId;
    public int rewardAmount = 1;

    [Header("Reward Type 2: Unlock Object")]
    [Tooltip("ลากประตู หรือ Stuff ที่ต้องการปลดล็อคมาใส่ตรงนี้")]
    public Stuff objectToUnlock;

    // --- ส่วนที่เพิ่มใหม่สำหรับ Type 3 ---
    [Header("Reward Type 3: Set Active Object")]
    [Tooltip("ลาก GameObject ที่อยากให้ เปิด หรือ ปิด มาใส่")]
    public GameObject objectToToggle;
    [Tooltip("ติ๊กถูก = สั่งเปิด (Active), ติ๊กออก = สั่งปิด (Inactive)")]
    public bool targetActiveState = true;
    // ----------------------------------

    [Header("Dialogs")]
    public string questDesc = "I need a key.";
    public string completeDesc = "Thank you!";
    public TMP_Text WordTextUI;

    // Internal Variables
    private Quest currentQuest;
    private ItemDefinition _requiredItemDef;
    private ItemDefinition _rewardItemDef;
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
            canTalk = false;
        }
    }

    public void Interact(Player player)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX("NPC");
        }

        if (!canTalk || _requiredItemDef == null) return;

        if (currentQuest.isCompleted)
        {
            ShowDialog(completeDesc);
            return;
        }

        if (currentQuest.isActive)
        {
            CheckAndCompleteQuest(player);
        }
        else
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
            playerInv.Remove(_requiredItemDef, requiredAmount);
            GiveReward(player); // เรียกฟังก์ชันให้รางวัล
            CompleteQuest(currentQuest);
        }
        else
        {
            ShowDialog($"I still need {_requiredItemDef.DisplayName}.\nYou have {count}/{requiredAmount}.");
        }
    }

    private void GiveReward(Player player)
    {
        switch (npcType)
        {
            case NPCType.GiveItem:
                if (_rewardItemDef != null)
                {
                    player.Inventory.Add(_rewardItemDef, rewardAmount);
                    ShowDialog($"Here is your {_rewardItemDef.DisplayName}!");
                }
                break;

            case NPCType.UnlockObject:
                if (objectToUnlock != null)
                {
                    objectToUnlock.isUnlock = true;
                    ShowDialog("The door is unlocked now!");
                }
                else
                {
                    Debug.LogWarning("[NPC] ลืมลาก objectToUnlock มาใส่ใน Inspector!");
                }
                break;

            // --- ส่วน Logic ใหม่ของ Type 3 ---
            case NPCType.SetActiveObject:
                if (objectToToggle != null)
                {
                    // สั่งเปิดหรือปิดตามที่ตั้งค่าไว้
                    objectToToggle.SetActive(targetActiveState);

                    // ปรับคำพูดให้เข้ากับสถานการณ์
                    string stateText = targetActiveState ? "activated" : "deactivated";
                    ShowDialog($"Mechanism has been {stateText}!");
                }
                else
                {
                    Debug.LogWarning("[NPC] ลืมลาก objectToToggle มาใส่ใน Inspector!");
                }
                break;
                // --------------------------------
        }
    }

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