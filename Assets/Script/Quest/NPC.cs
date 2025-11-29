using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : Stuff, IInteractable, IQuestGiver
{
    [Header("Quest Settings")]
    [Tooltip("ใส่ ID ให้ตรงกับใน InventoryManagement เช่น 'key_01'")]
    public string questItemId; // เปลี่ยนจาก ItemDefinition เป็น string
    public int itemAmountNeeded = 1;
    public string questTitle = "Find the Key";
    public string questDesc = "Please bring me the Key.";

    [Header("UI References")]
    public TMP_Text WordTextUI;

    private Quest currentQuest;
    private ItemDefinition _targetItemDef; // ตัวแปรเก็บข้อมูลไอเท็มตัวจริงที่ Lookup มา
    public bool canTalk = true;
    public bool isInteractable { get => canTalk; set => canTalk = value; }

    public override void SetUP()
    {
        base.SetUP();
        if (WordTextUI != null) WordTextUI.gameObject.SetActive(false);

        // --- เพิ่มส่วน Lookup ตรงนี้ ---
        var im = InventoryManagement.Instance;
        if (im != null && !string.IsNullOrEmpty(questItemId))
        {
            _targetItemDef = im.FindById(questItemId);

            if (_targetItemDef == null)
            {
                Debug.LogError($"[NPC] หาไอเท็ม ID: '{questItemId}' ไม่เจอใน InventoryManagement!");
                return; // จบการทำงานถ้าหาของไม่เจอ
            }
        }
        else
        {
            Debug.LogWarning("[NPC] ยังไม่ได้ตั้งค่า InventoryManagement หรือ questItemId");
            return;
        }
        // -----------------------------

        // สร้าง Quest โดยใช้ _targetItemDef ตัวจริงที่หามาได้
        currentQuest = new Quest(questTitle, questDesc, _targetItemDef, itemAmountNeeded);
    }

    public void Interact(Player player)
    {
        // ... (Logic ส่วนนี้เหมือนเดิมได้เลย) ...

        // 1. ถ้าเควสจบไปแล้ว
        if (currentQuest.isCompleted)
        {
            ShowDialog("Thank you for your help!");
            return;
        }

        // 2. ถ้ายังไม่เคยรับเควส
        if (!currentQuest.isActive)
        {
            StartQuest(currentQuest);
            ShowDialog(currentQuest.description + $"\n(Need: {currentQuest.requiredAmount} {_targetItemDef.DisplayName})");
            return;
        }

        // 3. ถ้ารับเควสแล้ว -> เช็คของ
        if (currentQuest.isActive)
        {
            CheckAndCompleteQuest(player);
        }
    }

    // ... (ส่วน Update Override ที่แก้ไปรอบที่แล้ว) ...
    public override void Update()
    {
        bool isTalking = WordTextUI.gameObject.activeSelf;
        if (isTalking)
        {
            if (interactionTextUI.gameObject.activeSelf)
                interactionTextUI.gameObject.SetActive(false);
            return;
        }
        base.Update();
    }

    private void CheckAndCompleteQuest(Player player)
    {
        if (_targetItemDef == null) return;

        Inventory playerInventory = player.Inventory;

        if (playerInventory != null)
        {
            // เช็คจำนวน (ตอนนี้ _targetItemDef คือตัวเดียวกับในกระเป๋า Player แล้ว)
            int currentAmount = playerInventory.CountOf(_targetItemDef);

            if (currentAmount >= currentQuest.requiredAmount)
            {
                bool removed = playerInventory.Remove(_targetItemDef, currentQuest.requiredAmount);
                if (removed)
                {
                    CompleteQuest(currentQuest);
                    ShowDialog("Wow! You found it! Thank you.");
                }
            }
            else
            {
                ShowDialog($"I need {_targetItemDef.DisplayName}.\nYou have {currentAmount}/{currentQuest.requiredAmount}.");
            }
        }
    }

    // ... (ฟังก์ชัน StartQuest, CompleteQuest, ShowDialog เหมือนเดิม) ...
    public void StartQuest(Quest quest) { quest.isActive = true; }
    public void CompleteQuest(Quest quest) { quest.isActive = false; quest.isCompleted = true; StartCoroutine(CloseDialogAfterDelay(3f)); }
    private void ShowDialog(string message) { WordTextUI.text = message; WordTextUI.gameObject.SetActive(true); }
    IEnumerator CloseDialogAfterDelay(float delay) { yield return new WaitForSeconds(delay); WordTextUI.gameObject.SetActive(false); }
    public bool CanGiveQuest() => !currentQuest.isActive;
}