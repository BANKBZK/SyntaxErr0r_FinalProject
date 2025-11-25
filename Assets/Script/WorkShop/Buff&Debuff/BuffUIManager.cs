using UnityEngine;
using UnityEngine.UI;

public class BuffUIManager : MonoBehaviour
{
    [Header("Dependencies")]
    public SkillBook skillBook;      // Drag the SkillBook into Inspector
    public BuffUIAssets buffAssets;  // Drag BuffData into this

    [Header("UI Elements")]
    public Transform effectParticles;
    public Image buffImage;

    private GameObject currentEffectObject;

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
            var entry = buffAssets.GetEntry(skill.skillName);
            // Take the image from Assets using the skill name.
            Sprite s = buffAssets.GetSprite(skill.skillName);

            if (s != null && entry != null)
            {
                {
                    ShowBuff(skill.timer, s);
                }

                if (entry.edgeScreenEffect != null)
                {
                    ShowEffect(entry.edgeScreenEffect);
                }
            }           
        }
    }

    public void ShowEffect(GameObject prefab)
    {
        if (currentEffectObject != null)
            Destroy(currentEffectObject);

        if (effectParticles != null)
        {            
            currentEffectObject = Instantiate(prefab, effectParticles);

            currentEffectObject.transform.localPosition = Vector3.zero;
            currentEffectObject.transform.localRotation = Quaternion.identity;
            currentEffectObject.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.Log("Forgot to drag EffectPosition into Effect Particles!");
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

            if (currentEffectObject != null)
            {
                Destroy(currentEffectObject);
                currentEffectObject = null;
            }
        }
    }
}