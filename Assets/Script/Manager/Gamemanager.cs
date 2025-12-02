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

    [Header("UI References")]
    [Tooltip("ลาก Slider หลอดเลือดมาใส่ (หรือตั้ง Tag 'HPBar' ให้ Slider เพื่อให้หาเจอเอง)")]
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

        // 3. จัดการเมาส์
        if (isGamePaused)
        {
            // ถ้าหยุดเกม -> โชว์เมาส์ให้กดเมนูได้
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // ถ้าเล่นต่อ -> ซ่อนเมาส์ (เฉพาะถ้าหน้าแพ้ไม่เปิดอยู่)
            if (looseUi == null || !looseUi.activeSelf)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    // ------------------- Gameplay APIs -------------------

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        // 1. ถ้าหา HP Bar ไม่เจอ (เช่น เพิ่งเปลี่ยนฉาก) ให้ลองค้นหาจาก Tag "HPBar"
        if (hpBar == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("HPBar");
            if (go != null) hpBar = go.GetComponent<Slider>();
        }

        // 2. ถ้ายังไม่เจออีก ก็จบข่าว (แปลว่าลืมวาง Slider หรือลืมติด Tag)
        if (hpBar == null) return;

        // 3. อัปเดตค่า
        hpBar.maxValue = maxHealth;
        hpBar.value = currentHealth;

        // 4. เช็คเงื่อนไขแพ้
        if (currentHealth <= 0)
        {
            if (looseUi != null && !looseUi.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                looseUi.SetActive(true);
                Time.timeScale = 0; // หยุดเกมเมื่อตาย
            }
        }
    }
}