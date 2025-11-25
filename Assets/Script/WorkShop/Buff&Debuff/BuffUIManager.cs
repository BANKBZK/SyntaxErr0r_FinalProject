using UnityEngine;
using UnityEngine.UI;

public class BuffUIManager : MonoBehaviour
{
    [Header("Dependencies")]
    public SkillBook skillBook;      // Inspector Drag the SkillBook into Inspector
    public BuffUIAssets buffAssets;  // Drag BuffData into this

    [Header("UI Elements")]
    public Image buffImage;

    float timer;
    bool isShowing = false;

    void OnEnable()
    {
        // If Skillbook told Useskill It will call OnSkilledUsed
        if (skillBook != null)
            skillBook.OnSkillActivated += OnSkillUsed;
    }

    void OnDisable()
    {
        // Cancel When Close the game or Change Scene (Error Protect)
        if (skillBook != null)
            skillBook.OnSkillActivated -= OnSkillUsed;
    }

    void Start()
    {
        buffImage.gameObject.SetActive(false);
    }

    void OnSkillUsed(Skill skill)
    {
        // check if it's a skill with a timer (if skill.timer <= 0, it means the skill is finished after pressing it, no need to show the buff)
        if (skill.timer > 0)
        {
            // Take the image from Assets using the skill name.
            Sprite s = buffAssets.GetSprite(skill.skillName);

            if (s != null)
            {
                ShowBuff(skill.timer, s);
            }
        }
    }

    public void ShowBuff(float duration, Sprite sprite)
    {
        buffImage.sprite = sprite;
        buffImage.gameObject.SetActive(true);
        timer = duration;
        isShowing = true;
    }

    void Update()
    {
        if (!isShowing) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            buffImage.gameObject.SetActive(false);
            isShowing = false;
        }
    }
}