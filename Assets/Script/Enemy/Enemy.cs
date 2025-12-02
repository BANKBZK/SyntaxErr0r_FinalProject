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

        // นำส่วนแก้ไข Damage ออกตามที่ขอครับ

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
        // ไม่ต้อง Reset Bool Attack แล้ว เพราะเราจะใช้ Trigger แทน
        Move(Vector3.zero);
    }

    private void ChasePlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        Turn(direction);
        Move(direction.normalized);
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
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
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
    }
}