using System;
using UnityEngine;

public class StrengthBuff : Skill
{
    float DamageIncrease = 10f;
    float OriginalDamage;
    float TargetDamage;

    public float Duration{ get; set; }

    public StrengthBuff()
    {
        this.skillName = "Increase Damage";
        this.cooldownTime = 10;
        this.Duration = 2f;
    }
    public override void Activate(Character character)
    {
        timer = Duration;

        OriginalDamage = character.attackDamage;
        TargetDamage = OriginalDamage + DamageIncrease;
        Debug.Log($"{character.Name} damage increased by {DamageIncrease} for {Duration} seconds.");
    }

    public override void Deactivate(Character character)
    {
        character.attackDamage = OriginalDamage;
        Debug.Log($"{character.Name}'s increase damage has ended.");
    }

    public override void UpdateSkill(Character character)
    {
        timer -= Time.deltaTime;
        character.attackDamage = TargetDamage;
        if (timer <= 0)
        {
            Deactivate(character);
        }
    }
}
