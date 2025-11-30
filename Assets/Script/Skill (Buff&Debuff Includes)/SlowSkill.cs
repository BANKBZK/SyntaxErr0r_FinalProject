using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SlowSkill", menuName = "Skills/SlowSkill")]
public class SlowSkill : Skill
{
    [Header("Skill Settings")]
    public float searchRadius = 5f;
    [Range(0, 1)]
    public float slowPercentage = 0.5f; // Slow 50%
    public float slowDuration = 3f; // ... วินาที

    public SlowSkill()
    {
        this.skillName = "Slow";
        this.cooldownTime = 6;
        this.isFollowPlayer = true;
    }

    public override void Activate(Character character)
    {
        Debug.Log($"{character.name} used {skillName} slowing enemies nearby!");

        Enemy[] targets = GetEnemysInRange(character);

        if (targets.Length > 0)
        {
            foreach (var enemy in targets)
            {
                enemy.ApplySlow(slowPercentage, slowDuration);
                Debug.Log($"Slowed {enemy.name} by {slowPercentage * 100}%");
            }
        }
    }

    private Enemy[] GetEnemysInRange(Character caster)
    {
        Collider[] hitColliders = Physics.OverlapSphere(caster.transform.position, searchRadius); 
        List <Enemy> Enemys = new List<Enemy>();

        foreach (var hitCollider in hitColliders)
        {
            Enemy targetCharacter = hitCollider.GetComponent<Enemy>();
            if(targetCharacter != null && targetCharacter != caster)
            {
                Enemys.Add(targetCharacter);
            }
        }
        return Enemys.ToArray();
    }

    public override void Deactivate(Character character)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateSkill(Character character)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
