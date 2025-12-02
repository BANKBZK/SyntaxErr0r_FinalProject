using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public Inventory Inventory { get; private set; }
    public UIInventory uiInventory;

    [Header("Hand setting")]
    public Transform RightHand;
    public Transform LeftHand;
    public AudioClip addItem;
    public AudioClip onAttack;

    Vector3 _inputDirection;
    bool _isAttacking = false;
    bool _isInteract = false;

    private void Awake()
    {
        // สร้าง Inventory เตรียมไว้ก่อน Start
        Inventory = new Inventory(10);
    }

    public override void Start()
    {
        base.Start(); // เรียก Character.Start() เพื่อ SetUP ค่าต่างๆ

        // ผูก Inventory เข้ากับ UI
        if (uiInventory != null) uiInventory.Bind(Inventory);

        // ✅ อัปเดตเลือดเริ่มเกมทันที
        GameManager.Instance?.UpdateHealthBar(health, maxHealth);
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
        if (Input.GetKeyDown(KeyCode.I)) uiInventory?.Toggle();
    }

    private void HandleInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        _inputDirection = new Vector3(x, 0, y);
        if (Input.GetMouseButtonDown(0))
        {
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
            if (e != null)
            {
                e.Interact(this);
            }
            _isInteract = false;
        }
    }

    // ------------------- Override ส่วนที่สำคัญ -------------------

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount); // ลดเลือดจริงในตัวแปร health

        // ✅ ส่งค่าไปอัปเดตหลอดเลือดที่ GameManager
        GameManager.Instance?.UpdateHealthBar(health, maxHealth);
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);

        // ✅ ส่งค่าไปอัปเดตหลอดเลือดที่ GameManager
        GameManager.Instance?.UpdateHealthBar(health, maxHealth);
    }
}