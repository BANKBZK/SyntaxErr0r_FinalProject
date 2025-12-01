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

        // ช็คทุกสกิลในกระเป๋าทุกครั้ง
        foreach (var skill in skillsSet)
        {
            // เช็คว่าช่องนั้นมีสกิลใส่ไว้ไหม (ป้องกัน Error ช่อง None)
            if (skill != null)
            {
                // รีเซ็ตเวลาใช้ล่าสุด
                skill.lastUsedTime = -9999f;

                // รีเซ็ต timer ด้วย (เผื่อค้าง)
                skill.timer = 0;
            }
        }
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
        if (index < 0 || index >= skillsSet.Count)
        {
            Debug.LogWarning($"Invalid skill index: {index}");
            return;
        }
        Skill skill = skillsSet[index];
        if (skill == null)
        {
            Debug.LogWarning($"Skill at index {index} is null.");
            return;
        }
        if (skillEffects == null || index >= skillEffects.Length || skillEffects[index] == null)
        {
            Debug.LogWarning($"Skill effect for index {index} is missing.");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Player reference is missing!");
            return;
        }
        if (!skill.IsReady(Time.time))
        {
            Debug.Log($"Skill '{skill.skillName}' is on cooldown.");
            return;
        }
        Vector3 spawnPosition = (castPoint != null) ? castPoint.position : transform.position;
        Quaternion spawnRotation = (castPoint != null) ? castPoint.rotation : transform.rotation;
        GameObject g = Instantiate(skillEffects[index], spawnPosition, spawnRotation);
        if (skill.isFollowPlayer)
        {
            g.transform.SetParent(castPoint != null ? castPoint : transform);
        }
        Destroy(g, 3);
        skill.Activate(player);
        if (skill.timer > 0)
        {
            OnSkillActivated?.Invoke(skill);
            DulationSkills.Add(skill);
        }
        skill.TimeStampSkill(Time.time);
    }

    private void OnDrawGizmos()
    {
        // Set the gizmo color
        Gizmos.color = Color.yellow;
        // Draw a wire sphere at the player's position with the fireball's search radius
        Gizmos.DrawWireSphere(transform.position, 5);
        
    }
}
