using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Item : Identity
{
    private Collider _collider;
    protected Collider itemCollider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
                _collider.isTrigger = true;
            }
            return _collider;
        }
    }

    public override void SetUP()
    {
        base.SetUP();
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    public Item() { }

    public Item(Item item)
    {
        this.Name = item.Name;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                OnCollect(player);
            }
        }
    }

    public virtual void OnCollect(Player player)
    {
        if (player.Inventory.AddItem(this))
        {
            Debug.Log($"Collected {Name}");
            Destroy(gameObject); // ลบไอเท็มจากโลกหลังเก็บ
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }

    public virtual void Use(Player player)
    {
        Debug.Log($"Using {Name}");
    }
}