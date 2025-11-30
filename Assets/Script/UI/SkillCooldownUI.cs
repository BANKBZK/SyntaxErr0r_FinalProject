using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("Data")]
    public Skill skillData;

    [Header("UI Components")]
    public Image cooldownOverlay;

    public void Start()
    {      
        cooldownOverlay.fillAmount = 0;      
    }

    public void Update()
    {
        if (skillData == null)
        {
            return;
        }

        // คำนวณเวลา: เวลาปัจจุบัน - เวลาที่กดใช้ไปล่าสุด
        float timePassed = Time.time - skillData.lastUsedTime;
        float cooldownRemains = skillData.cooldownTime - timePassed;

        // เช็คว่ายังติดคูลดาวน์อยู่หรือป่าว
        if (cooldownRemains > 0)

        {
            // --- ช่วงติดคูลดาวน์ ---


            // หมุนวงกลมดำๆ
            cooldownOverlay.fillAmount = cooldownRemains / skillData.cooldownTime;
        }
        else
        {
            // --- พร้อมใช้งาน เอาตัว progress bar ออก---
            cooldownOverlay.fillAmount = 0;
        }
    }
}
