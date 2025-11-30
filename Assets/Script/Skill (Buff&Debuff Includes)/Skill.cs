using System;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public float cooldownTime;
    public float lastUsedTime = float.MinValue; // àÇÅÒÅèÒÊØ´·ÕèãªéÊ¡ÔÅ
    public float timer; // µÑÇ¨ÑºàÇÅÒÊÓËÃÑºÊ¡ÔÅ·ÕèÁÕ¼ÅµèÍà¹×èÍ§

    public bool isFollowPlayer; // ติ๊กถูก = effect ตาม || ไม่ติ๊ก = ไม่ตาม

    // àÁ¸Í´·Õèà»ç¹ abstract, ºÑ§¤ÑºãËé¤ÅÒÊÅÙ¡µéÍ§ implement
    public abstract void Activate(Character character);
    public abstract void Deactivate(Character character);
    public abstract void UpdateSkill(Character character);
    public void ResetCooldown()
    {
        lastUsedTime = float.MinValue; // àÇÅÒÅèÒÊØ´·ÕèãªéÊ¡ÔÅ
    }
    public bool IsReady(float GameTime)
    {
        return GameTime >= lastUsedTime + cooldownTime;
    }

    // àÁ¸Í´ÊÓËÃÑººÑ¹·Ö¡àÇÅÒ·ÕèãªéÊ¡ÔÅ
    public void TimeStampSkill(float GameTime)
    {
        lastUsedTime = GameTime;
    }

    // àÁ¸Í´·ÕèÁÕ¡ÒÃãªé§Ò¹ÃèÇÁ¡Ñ¹
    public void DisplayInfo()
    {
        Debug.Log($"Skill: {skillName}");
        Debug.Log($"Cooldown: {cooldownTime}s");
    }
}