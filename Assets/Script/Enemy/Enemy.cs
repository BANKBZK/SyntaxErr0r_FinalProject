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
    [SerializeField] private float turnSpeed = 15f; // ✅ เพิ่มตัวแปรนี้เพื่อแก้ปัญหาหันช้า

    [Header("Obstacle Avoidance")]
    [Tooltip("ระยะเช็คสิ่งกีดขวาง")]
    [SerializeField] private float obstacleCheckDistance = 4.5f; // ✅ เพิ่มระยะให้เห็นล่วงหน้าเร็วขึ้น
    [Tooltip("เลเยอร์ของสิ่งกีดขวาง (เช่น Wall, Tree)")]
    [SerializeField] private LayerMask obstacleLayer;
    [Tooltip("มุมของหนวดแมวซ้ายขวา (องศา)")]
    [SerializeField] private float whiskerAngle = 45f; // ✅ เพิ่มมุมให้หันหลบชัดเจนขึ้น

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

        // 2. คำนวณทิศทางใหม่เพื่อหลบสิ่งกีดขวาง (Whiskers Logic)
        Vector3 finalDirection = GetDirectionWithAvoidance(desiredDirection);

        // 3. สั่งเดินตามทิศทางใหม่
        Turn(finalDirection);
        Move(finalDirection);
    }

    // ✅ ฟังก์ชันคำนวณการหลบหลีก
    private Vector3 GetDirectionWithAvoidance(Vector3 targetDir)
    {
        Vector3 startPos = transform.position + Vector3.up * 0.5f;

        // ยิง Ray ตรงกลาง
        bool hitFront = Physics.Raycast(startPos, transform.forward, obstacleCheckDistance, obstacleLayer);

        if (hitFront)
        {
            // ถ้าข้างหน้าตัน ให้ลองเช็คซ้ายขวา
            Vector3 leftDir = Quaternion.Euler(0, -whiskerAngle, 0) * transform.forward;
            Vector3 rightDir = Quaternion.Euler(0, whiskerAngle, 0) * transform.forward;

            bool hitLeft = Physics.Raycast(startPos, leftDir, obstacleCheckDistance, obstacleLayer);
            bool hitRight = Physics.Raycast(startPos, rightDir, obstacleCheckDistance, obstacleLayer);

            if (!hitLeft && !hitRight)
            {
                // ✅ ถ้าว่างทั้งคู่ ให้เลือกทางที่ "ใกล้เคียงกับทิศทางผู้เล่น" มากที่สุด
                // (แบบเดิมคือบังคับขวา ทำให้บางทีมันเดินอ้อมโลก)
                float dotLeft = Vector3.Dot(leftDir, targetDir);
                float dotRight = Vector3.Dot(rightDir, targetDir);

                return dotLeft > dotRight ? leftDir : rightDir;
            }
            else if (!hitLeft)
            {
                return leftDir; // ซ้ายว่าง ไปซ้าย
            }
            else if (!hitRight)
            {
                return rightDir; // ขวาว่าง ไปขวา
            }
            else
            {
                // ถ้าตันหมด ให้หันขวา 90 องศาเลย (หักหลบแรงๆ)
                return Quaternion.Euler(0, 90, 0) * transform.forward;
            }
        }

        // ถ้าข้างหน้าไม่ตัน ก็ไปตามทางเดิมที่อยากไป (หา Player)
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(startPos, startPos + transform.forward * obstacleCheckDistance);

        Gizmos.color = Color.blue;
        Vector3 leftDir = Quaternion.Euler(0, -whiskerAngle, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, whiskerAngle, 0) * transform.forward;
        Gizmos.DrawLine(startPos, startPos + leftDir * obstacleCheckDistance);
        Gizmos.DrawLine(startPos, startPos + rightDir * obstacleCheckDistance);
    }
}