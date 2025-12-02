using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    protected enum State { idel, cheses, attack, death }

    [Header("Combat Stats")]
    [SerializeField]
    private float TimeToAttack = 1f;
    [SerializeField]
    private float detectionRange = 5f;
    [SerializeField]
    private float attackRange = 1.5f;
    [Tooltip("ความเร็วในการหันหน้า (ยิ่งเยอะยิ่งหันไว)")]
    [SerializeField] private float turnSpeed = 15f;

    [Header("Obstacle Avoidance")]
    [Tooltip("ระยะเช็คสิ่งกีดขวางด้านหน้า")]
    [SerializeField] private float obstacleCheckDistance = 4.5f;
    [Tooltip("ระยะเช็คสิ่งกีดขวางด้านข้าง (ควรสั้นกว่าด้านหน้านิดหน่อย)")]
    [SerializeField] private float sideCheckDistance = 3.5f; // ✅ เพิ่มตัวแปรนี้
    [Tooltip("เลเยอร์ของสิ่งกีดขวาง (เช่น Wall, Tree)")]
    [SerializeField] private LayerMask obstacleLayer;
    [Tooltip("มุมของหนวดแมวซ้ายขวา (องศา)")]
    [SerializeField] private float whiskerAngle = 45f;

    protected State currentState = State.idel;
    protected float timer = 0f;

    private float originalSpeed;
    private Coroutine slowCoroutine;

    private SkinnedMeshRenderer _meshRenderer;
    private Color _originalColor;

    public override void Start()
    {
        base.Start();
        originalSpeed = movementSpeed;

        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (_meshRenderer != null)
        {
            if (_meshRenderer.material.HasProperty("_BaseColor"))
                _originalColor = _meshRenderer.material.GetColor("_BaseColor");
            else
                _originalColor = _meshRenderer.material.color;
        }
    }

    public void ApplySlow(float percentage, float duration)
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            ResetColor();
        }
        slowCoroutine = StartCoroutine(SlowProcess(percentage, duration));
    }

    IEnumerator SlowProcess(float percentage, float duration)
    {
        movementSpeed = 0.5f;

        if (_meshRenderer != null) _meshRenderer.material.color = Color.cyan;

        yield return new WaitForSeconds(duration);

        movementSpeed = originalSpeed;
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

        timer -= Time.deltaTime;

        float distance = GetDistanPlayer();

        if (distance > detectionRange)
        {
            StopBehavior();
        }
        else if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            PerformAttack();
        }
    }

    private void StopBehavior()
    {
        Move(Vector3.zero);
    }

    private void ChasePlayer()
    {
        // 1. หาความต้องการเดิม (วิ่งเข้าหา Player ตรงๆ)
        Vector3 desiredDirection = (player.transform.position - transform.position).normalized;

        // 2. คำนวณทิศทางใหม่เพื่อหลบสิ่งกีดขวาง (Whiskers Logic แบบใหม่)
        Vector3 finalDirection = GetDirectionWithAvoidance(desiredDirection);

        // 3. สั่งเดินตามทิศทางใหม่
        Turn(finalDirection);
        Move(finalDirection);
    }

    // ✅ ฟังก์ชันคำนวณการหลบหลีก (ปรับปรุงใหม่ แก้ปัญหาเดินส่าย)
    private Vector3 GetDirectionWithAvoidance(Vector3 targetDir)
    {
        Vector3 startPos = transform.position + Vector3.up * 0.5f;

        // ทิศทางของหนวดแมว
        Vector3 forward = transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -whiskerAngle, 0) * forward;
        Vector3 rightDir = Quaternion.Euler(0, whiskerAngle, 0) * forward;

        // ยิง Ray 3 เส้นเสมอ
        bool hitFront = Physics.Raycast(startPos, forward, obstacleCheckDistance, obstacleLayer);
        bool hitLeft = Physics.Raycast(startPos, leftDir, sideCheckDistance, obstacleLayer);
        bool hitRight = Physics.Raycast(startPos, rightDir, sideCheckDistance, obstacleLayer);

        if (hitFront)
        {
            // --- กรณีข้างหน้าตัน (ต้องเลี้ยวหลบ) ---
            if (!hitLeft && !hitRight)
            {
                // ว่างทั้งคู่ -> ไปทางที่ใกล้ Player ที่สุด
                return Vector3.Dot(leftDir, targetDir) > Vector3.Dot(rightDir, targetDir) ? leftDir : rightDir;
            }
            else if (!hitLeft) return leftDir; // ซ้ายว่างไปซ้าย
            else if (!hitRight) return rightDir; // ขวาว่างไปขวา
            else
            {
                // ตันทุกทาง -> กลับหลังหัน หรือหักหลบ 90 องศา
                return Quaternion.Euler(0, 90, 0) * forward;
            }
        }
        else
        {
            // --- กรณีข้างหน้าโล่ง (แต่วัดใจด้านข้างด้วย) ---
            // ปัญหาเดิมคือพอหน้าโล่ง มันรีบเลี้ยวกลับหา Player เลยทำให้ไหล่ไปชนกำแพง
            // วิธีแก้: ถ้าด้านข้างยังติดกำแพงอยู่ ให้ "ดัน" ตัวเองออกห่างจากกำแพงนั้น

            Vector3 avoidancePush = Vector3.zero;

            if (hitLeft)
            {
                // ชนซ้าย -> ดันไปขวา (ใช้ transform.right)
                avoidancePush += transform.right;
            }
            if (hitRight)
            {
                // ชนขวา -> ดันไปซ้าย (ใช้ -transform.right)
                avoidancePush -= transform.right;
            }

            if (avoidancePush != Vector3.zero)
            {
                // เอาทิศทางที่อยากไป (targetDir) ผสมกับ แรงผลักหนีกำแพง (avoidancePush)
                // ยิ่งคูณเยอะ ยิ่งผลักแรง (เช่น 1.5f)
                return (targetDir + avoidancePush * 1.5f).normalized;
            }
        }

        // ถ้าโล่งหมดทุกทาง ก็ไปหา Player ตรงๆ
        return targetDir;
    }

    private void PerformAttack()
    {
        Move(Vector3.zero);

        Vector3 direction = player.transform.position - transform.position;
        Turn(direction);

        Attack(player);
    }

    protected override void Turn(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // ✅ ใช้ turnSpeed ที่ปรับเพิ่มขึ้น แทนค่าคงที่ 5f
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
    }

    protected virtual void Attack(Player _player)
    {
        if (timer <= 0)
        {
            _player.TakeDamage(Damage);

            animator.SetTrigger("Attack");

            Debug.Log($"{Name} attacks {_player.Name} for {Damage} damage.");

            timer = TimeToAttack;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // ✅ วาดเส้น Whiskers ให้เห็นใน Editor
        Vector3 startPos = transform.position + Vector3.up * 0.5f;

        // เส้นหน้า (สีฟ้า)
        Gizmos.color = hitFrontGizmo ? Color.red : Color.cyan;
        Gizmos.DrawLine(startPos, startPos + transform.forward * obstacleCheckDistance);

        // เส้นข้าง (สีน้ำเงิน) - คำนวณใหม่เพื่อให้วาดถูกต้อง
        Vector3 leftDir = Quaternion.Euler(0, -whiskerAngle, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, whiskerAngle, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(startPos, startPos + leftDir * sideCheckDistance);
        Gizmos.DrawLine(startPos, startPos + rightDir * sideCheckDistance);
    }

    // ตัวแปรสำหรับ Debug Gizmos (ไม่ส่งผลต่อเกม)
    private bool hitFrontGizmo = false;
}