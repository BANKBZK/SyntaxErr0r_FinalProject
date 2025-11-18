using UnityEditor.Analytics;
using UnityEngine;

public class Keypad : Stuff,IInteractable
{
    public bool isInteractable { get => isLock; set => isLock = value; }
    public GameObject keyPadUI;

    bool isOpen;

    public void Interact(Player player)
    {
        isOpen = !isOpen;
        keyPadUI.SetActive(isOpen);
    }

}
