using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)] // ให้ตื่นก่อน object อื่น ๆ ใน Scene
public sealed class GameManager : MonoBehaviour
{
    // ✅ Singleton (Instance)
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentScore = 0;

    [SerializeField] private Slider hpBar;

    public bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject looseUi;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // ✅ คงอยู่ข้ามซีน
        DontDestroyOnLoad(gameObject);

        // (ไม่บังคับ) ตรวจ refs สำคัญและเตือน
        if (hpBar == null) Debug.LogWarning("[GameManager] HPBar is not assigned.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySFX("Pause");
            }
        }
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;

        // 1. หยุด/เดิน เวลา
        Time.timeScale = isGamePaused ? 0 : 1;

        // 2. เปิด/ปิด หน้าต่าง UI
        if (pauseMenuUI != null) pauseMenuUI.SetActive(isGamePaused);

        // 3. ✅ จัดการเมาส์ (เพิ่มใหม่)
        if (isGamePaused)
        {
            // ถ้าหยุดเกม -> โชว์เมาส์ให้กดเมนูได้
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // ถ้าเล่นต่อ -> ซ่อนเมาส์และล็อคไว้กลางจอ
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // ------------------- Gameplay APIs -------------------

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (hpBar == null) return;
        hpBar.maxValue = maxHealth;
        hpBar.value = currentHealth;
        if(currentHealth <= 0)
        {
            if (looseUi != null)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                looseUi.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

}