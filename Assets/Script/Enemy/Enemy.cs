using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    protected enum State { idel, cheses, attack, death }

    [Header("Combat Stats")]
    [SerializeField]
    private float TimeToAttack = 1f;
    [SerializeField]
    private float detectionRange = 5f; // ระยะมองเห็น (ถ้า Player อยู่ไกลกว่านี้จะไม่ตาม)
    [SerializeField]
    private float attackRange = 1.5f;  // ระยะโจมตี (ถ้า Player เข้ามาในระยะนี้จะเริ่มตี)

    protected State currentState = State.idel;
    protected float timer = 0f;

    private float originalSpeed;
    private Coroutine slowCoroutine;

    // เพิ่มตัวแปรสำหรับ Visual Effect (เปลี่ยนสีตัว)
    private SkinnedMeshRenderer _meshRenderer;
    private Color _originalColor;

    public override void Start()
    {
        base.Start(); // เรียกตัวแม่ Character ให้หา Rigidbody ให้
        originalSpeed = movementSpeed; // จำความเร็วเริ่มต้นจากตัวแม่

        // หา Mesh เพื่อเก็บสีเดิมไว้ (รองรับทั้ง Standard และ URP)
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (_meshRenderer != null)
        {
            // ตรวจสอบชื่อ Property ของสี (URP ใช้ _BaseColor, Built-in ใช้ _Color)
            if (_meshRenderer.material.HasProperty("_BaseColor"))
                _originalColor = _meshRenderer.material.GetColor("_BaseColor");
            else
                _originalColor = _meshRenderer.material.color;
        }
    }

    public void ApplySlow(float percentage, float duration)
    {
        // ถ้าโดนสโลว์ซ้ำ ให้รีเซ็ตเวลาใหม่ (Stop ตัวเก่า Start ตัวใหม่)
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            // คืนค่าสีก่อนเริ่มใหม่ (กันสีเพี้ยน)
            ResetColor();
        }
        slowCoroutine = StartCoroutine(SlowProcess(percentage, duration));
    }

    IEnumerator SlowProcess(float percentage, float duration)
    {
        // 1. ลดความเร็ว (ปรับให้เหลือ 0.5 ทันทีตามที่ขอ)
        movementSpeed = 0.5f;

        // 2. เปลี่ยนสีตัวเป็นสีฟ้า (Visual Feedback)
        if (_meshRenderer != null) _meshRenderer.material.color = Color.cyan;

        yield return new WaitForSeconds(duration);

        // 3. คืนค่าความเร็วเดิม
        movementSpeed = originalSpeed;

        // 4. คืนค่าสีเดิม
        ResetColor();

        slowCoroutine = null;
    }

    private void ResetColor()
    {
        if (_meshRenderer != null) _meshRenderer.material.color = _originalColor;
    }

    private void Update()
    {
        if (player == null)
        {
            StopBehavior();
            return;
        }

        // นับถอยหลัง Timer ตลอดเวลา
        timer -= Time.deltaTime;

        // เช็คระยะห่าง
        float distance = GetDistanPlayer();

        // 1. ถ้าระยะห่าง มากกว่า ระยะมองเห็น -> หยุดนิ่ง (มองไม่เห็น)
        if (distance > detectionRange)
        {
            StopBehavior();
        }
        // 2. ถ้าเห็นแล้ว แต่อยู่ไกลกว่าระยะโจมตี -> เดินไล่ล่า
        else if (distance > attackRange)
        {
            ChasePlayer();
        }
        // 3. ถ้าอยู่ในระยะโจมตี -> หยุดเดินแล้วโจมตี
        else
        {
            PerformAttack();
        }
    }

    private void StopBehavior()
    {
        animator.SetBool("Attack", false);
        Move(Vector3.zero); // หยุดเดิน
        // ไม่สั่ง Turn() ให้ยืนหันทางเดิม
    }

    private void ChasePlayer()
    {
        animator.SetBool("Attack", false);

        Vector3 direction = player.transform.position - transform.position;
        Turn(direction); // หันหน้าหา Player
        Move(direction.normalized); // เดินเข้าหา
    }

    private void PerformAttack()
    {
        Move(Vector3.zero); // หยุดเดินเพื่อฟัน (ไม่ให้ตัวไหล)

        Vector3 direction = player.transform.position - transform.position;
        Turn(direction); // หันหน้าตาม Player เสมอ (เผื่อผู้เล่นเดินวน)

        Attack(player); // เรียก Logic การโจมตี
    }

    protected override void Turn(Vector3 direction)
    {
        // ป้องกัน Error กรณี direction เป็น 0
        if (direction != Vector3.zero)
        {
            // ล็อคแกน Y เพื่อไม่ให้ศัตรูหน้าทิ่มลงดินหรือเงยหน้าขึ้นฟ้า
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // หมุนแบบนุ่มนวล
        }
    }

    protected virtual void Attack(Player _player)
    {
        // เงื่อนไข: ถ้า Timer หมดเวลา (Cooldown เสร็จ) ให้โจมตี
        if (timer <= 0)
        {
            _player.TakeDamage(Damage);
            animator.SetBool("Attack", true);
            // Debug.Log($"{Name} attacks {_player.Name} for {Damage} damage.");

            // รีเซ็ตเวลา (รอ TimeToAttack วินาทีก่อนฟันครั้งถัดไป)
            timer = TimeToAttack;
        }
    }

    // วาดเส้นวงกลมใน Editor เพื่อให้เห็นระยะมองเห็นและระยะโจมตีชัดเจน
    private void OnDrawGizmosSelected()
    {
        // สีเหลือง = ระยะมองเห็น
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // สีแดง = ระยะโจมตี
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}