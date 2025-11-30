using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using System;

public class SkillBook : MonoBehaviour
{
    public Transform castPoint; // ไว้กำหนดพวกจุดเกิด Prefab
    public event Action<Skill> OnSkillActivated;
    public List<Skill> skillsSet = new List<Skill>();
    public GameObject[] skillEffects;
    List<Skill> DulationSkills = new List<Skill>();

    Player player;
    public void Start()
    {
        // à¾ÔèÁÊ¡ÔÅµèÒ§æ à¢éÒä»ã¹ List
        player = GetComponent<Player>();

        skillsSet.Add(new FireballSkill());
        skillsSet.Add(new HealSkill());
        skillsSet.Add(new BuffSkillMoveSpeed());
        skillsSet.Add(new StrengthBuff());
        skillsSet.Add(new SlowSkill());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill(0); // ãªéÊ¡ÔÅ·Õè 1 (Fireball)
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseSkill(1); // ãªéÊ¡ÔÅ·Õè 2 (Heal)
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseSkill(2); // ãªéÊ¡ÔÅ·Õè 3 (Buff Move Speed)
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseSkill(3); // (Attack Damage Buff)
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UseSkill(4); // Slow Enemy(S)
        }
            // ÍÑ»à´µÊ¡ÔÅ·ÕèÁÕ¼ÅµèÍà¹×èÍ§
            for (int i = DulationSkills.Count - 1; i >= 0; i--)
            {
                DulationSkills[i].UpdateSkill(player);
                if (DulationSkills[i].timer <= 0)
                {
                    DulationSkills.RemoveAt(i);
                }
            }
    }

    public void UseSkill(int index)
    {
        if (index >= 0 && index < skillsSet.Count)
        {
            Skill skill = skillsSet[index];
        
            if (!skill.IsReady(Time.time))
            {
                Debug.Log($"Skill '{skill.skillName}' is on cooldown. Time remaining: {skill.lastUsedTime + skill.cooldownTime - Time.time:F2}s");
                return; // ¨º¡ÒÃ·Ó§Ò¹¶éÒÊ¡ÔÅµÔ´¤ÙÅ´ÒÇ¹ì
            }
            Vector3 spawnPosition = (castPoint != null) ? castPoint.position : transform.position;
            Quaternion spawnRotation = (castPoint != null) ? castPoint.rotation : transform.rotation;
            GameObject g = Instantiate(skillEffects[index], spawnPosition, spawnRotation);

            if (skill.isFollowPlayer)
            {
                if (castPoint != null)
                {
                    g.transform.SetParent(castPoint);
                }
                else
                {
                    g.transform.SetParent(transform);
                }
            }
            Destroy(g, 3);
            skill.Activate(player);
            if(skill.timer > 0)
            OnSkillActivated?.Invoke(skill);
            skill.TimeStampSkill(Time.time); // ºÑ¹·Ö¡àÇÅÒ·ÕèãªéÊ¡ÔÅ
            // µÃÇ¨ÊÍºÇèÒà»ç¹Ê¡ÔÅ·ÕèÁÕ¼ÅµèÍà¹×èÍ§ËÃ×ÍäÁè
            if (skill.timer > 0)
            {
                DulationSkills.Add(skill);
            }
        }
    }
    private void OnDrawGizmos()
    {
        // Set the gizmo color
        Gizmos.color = Color.yellow;
        // Draw a wire sphere at the player's position with the fireball's search radius
        Gizmos.DrawWireSphere(transform.position, 5);
        
    }
}
