using UnityEngine;

public class DoorRotate : Stuff, IInteractable
{
    public DoorRotate()
    {
        Name = "DoorRotate";
    }
    private bool isOpen = true;
    public Animator doorAnimation;

    public bool isInteractable { get => isUnlock; set => isUnlock = value; }

    public void Interact(Player player)
    {
        if (!isUnlock)
        {
            Debug.Log("The door is locked.");
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySFX("DoorLock");
            }
            return;
        }
        if (isOpen)
        {
            doorAnimation.SetBool("DoorOpen", isOpen);
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySFX("DoorOpen");
            }
        }
        else
        {
            doorAnimation.SetBool("DoorOpen", isOpen);
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySFX("DoorClose");
            }
        }
        isOpen = !isOpen;
    }

}
