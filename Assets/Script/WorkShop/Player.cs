using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public Inventory Inventory { get; private set; }
    public UIInventory uiInventory;

    [Header("Hand setting")]
    public Transform RightHand;
    public Transform LeftHand;
    public List<Item> inventory = new List<Item>();
    public AudioClip addItem; //Here
    public AudioClip onAttack; //Here

    Vector3 _inputDirection;
    bool _isAttacking = false;
    bool _isInteract = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = maxHealth;
    }

    public void FixedUpdate()
    {
        Move(_inputDirection);
        Turn(_inputDirection);
        Attack(_isAttacking);
        Interact(_isInteract);
    }
    public void Update()
    {
        HandleInput();
        if (Input.GetKeyDown(KeyCode.I)) // กด I เพื่อเปิด Inventory
        {
            uiInventory.UpdateInventoryUI(Inventory);
        }
    }
    public void AddItem(Item item) 
    {
        inventory.Add(item);
        SoundManager.instance.PlaySFX(addItem); //Here
    }
    
    private void HandleInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        _inputDirection = new Vector3(x, 0, y);
        if (Input.GetMouseButtonDown(0)) {
            _isAttacking = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _isInteract = true;
        }

    }
    public void Attack(bool isAttacking) 
    {
        if (isAttacking) 
        {
            SoundManager.instance.PlaySFX(onAttack); //Here
            animator.SetTrigger("Attack");
            var e = InFront as Idestoryable;
            if (e != null)
            {
                e.TakeDamage(Damage);
                
                Debug.Log($"{gameObject.name} attacks for {Damage} damage.");
            }
            _isAttacking = false;
        }
    }
    private void Interact(bool interactable)
    {
        if (interactable)
        {
            IInteractable e = InFront as IInteractable;
            if (e != null) {
                e.Interact(this);
            }
            _isInteract = false;

        }
    }
    
    private void Awake()
    {
        Inventory = new Inventory(10); // กำหนดความจุเริ่มต้น
    }

    //à¾ÔèÁàµÔÁ¿Ñ§¡ìªÑ¹¡ÒÃÃÑ¡ÉÒáÅÐÃÑº¤ÇÒÁàÊÕÂËÒÂ
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        GameManager.instance.UpdateHealthBar(health, maxHealth);
    }
    public override void Heal(int amount)
    {
        base.Heal(amount);
        GameManager.instance.UpdateHealthBar(health, maxHealth);
    }

}
