using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    protected enum State { idel, cheses, attack, death }

    [SerializeField]
    private float TimeToAttack = 1f;
    protected State currentState = State.idel;
    protected float timer = 0f;

    private float originalSpeed;
    private Coroutine slowCoroutine;

    public override void Start()
    {
        base.Start(); // เรียกตัวแม่ Character ให้หา Rigidbody ให้

        originalSpeed = movementSpeed; // จำความเร็วเริ่มต้นจากตัวแม่
    }
    public void ApplySlow(float percentage, float duration)
    {
        if(slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = StartCoroutine(SlowProcess(percentage, duration));
    }

    IEnumerator SlowProcess(float percentage, float duration)
    {
        // ลดความเร็วเคลื่อนที่ตัวแปรของตัวแม่ตรงๆ
        movementSpeed = originalSpeed * (1f - percentage);
        Debug.Log($"{name} speed reduced to: {movementSpeed}");

        yield return new WaitForSeconds(duration);

        // เอาความเร็วปกติคืน ตอนครบเวลา
        movementSpeed = originalSpeed;
        Debug.Log("${name} speed restored");

        slowCoroutine = null;
    }

    private void Update()
    {
        if (player == null)
        {
            animator.SetBool("Attack", false);
            return;
        }

        Turn(player.transform.position - transform.position);
        timer -= Time.deltaTime;

        if (GetDistanPlayer() < 1.5)
        {
            Attack(player);
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }

    
    protected override void Turn(Vector3 direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }
    protected virtual void Attack(Player _player) {
        if (timer <= 0)
        {
            _player.TakeDamage(Damage);
            animator.SetBool("Attack", true);
            Debug.Log($"{Name} attacks {_player.Name} for {Damage} damage.");
            timer = TimeToAttack;
        }
        
    }
}
