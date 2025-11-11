using UnityEngine;

public class Trap : Stuff, IInteractable
{
    public Trap() {
        Name = "Trap";
    }
    public bool isInteractable { get => isLock; set => isLock = value; }
    public int Damage = 10;
    public GameObject spikes;
    public void Interact(Player player)
    {
        _collider.enabled = false;
        spikes.SetActive(false);
        isLock = false;
        Debug.Log("Trap Inactivated!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.TakeDamage(10);
        }
    }
}
