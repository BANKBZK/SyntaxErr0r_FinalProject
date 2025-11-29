using System;
using System.Collections;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class NPC : Stuff, IInteractable, IQuestGiver
{
    public bool canTalk = true;
    bool isGive = false;
    int talkTime = 3;
    int count = 0;
    public TMP_Text WordTextUI;
    private Quaternion newRotation;
    private Rigidbody rb;
    private bool canGiveQuest = false;
    bool isOn;
    

    public bool isInteractable { get => canTalk; set => canTalk = value; }
    Quest quest;
    public override void SetUP()
    {
        base.SetUP();
        WordTextUI.gameObject.SetActive(false);
        quest = new Quest("Kill Slime", "Kill 3 Slime");
    }
    public void Interact(Player player)
    {
        isOn = !isOn;
        if(isOn)
        {
            if (CanGiveQuest())
            {
                WordTextUI.text = quest.description;
                WordTextUI.gameObject.SetActive(true);
                isGive = true;
                StartQuest(quest);
            }
            if(!isGive)
            {
                WordTextUI.text = "Thank you for helping me!";
                WordTextUI.gameObject.SetActive(true);
                StartCoroutine(GiveQuestCooldown());
            }
        }
    }

    IEnumerator GiveQuestCooldown()
    {
        yield return new WaitForSeconds(1f);
        canGiveQuest = true;
        isOn = false;
        WordTextUI.text = "Thank you for helping me! \n(PRESS E)";
    }
    public void CompleteQuest(Quest quest)
    {
    }

    public void StartQuest(Quest quest)
    {
      
    }

    public bool CanGiveQuest() => canGiveQuest;
}
