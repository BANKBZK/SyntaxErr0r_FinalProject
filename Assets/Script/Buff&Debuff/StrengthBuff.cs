using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StrengthBuff", menuName = "Skills/StrengthBuff")]
public class StrengthBuff : Skill
{
    [Header("Skill Settings")]
    public int DamageIncrease = 10;
    public int OriginalDamage;
    public int TargetDamage;
    public float Duration { get; set; }
    public StrengthBuff()
    {
        this.skillName = "StrengthBuff";
        this.cooldownTime = 10;
        this.Duration = 5f;
        this.isFollowPlayer = true;
    }

    public override void Activate(Character character)
    {
        timer = Duration;

        OriginalDamage = character.Damage;
        TargetDamage = OriginalDamage + DamageIncrease;
        Debug.Log($"{character.Name} damage increase by {DamageIncrease} for {Duration} seconds");
    }

    public override void Deactivate(Character character)
    {
        character.Damage = OriginalDamage;
        Debug.Log($"{character.Name}'s increase damage has ended");
    }
        public override void UpdateSkill(Character character) 
    { 
        timer -= Time.deltaTime; 
        character.Damage = TargetDamage; 
        if (timer <= 0) 
        { Deactivate(character); 
        }
    }
}
