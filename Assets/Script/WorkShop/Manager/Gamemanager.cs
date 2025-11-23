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
    public bool isGamePaused = false;

    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Slider hpBar;

    [Header("Audio")]
    [SerializeField] private AudioClip pauseSound;

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
        if (scoreText == null) Debug.LogWarning("[GameManager] ScoreText is not assigned.");
        if (pauseMenuUI == null) Debug.LogWarning("[GameManager] PauseMenuUI is not assigned.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            if (pauseSound != null && SoundManager.instance != null)
                SoundManager.instance.PlaySFX(pauseSound);
        }
    }

    // ------------------- Gameplay APIs -------------------

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (hpBar == null) return;
        hpBar.maxValue = maxHealth;
        hpBar.value = currentHealth;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        if (scoreText != null)
            scoreText.text = currentScore.ToString();
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0 : 1;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(isGamePaused);
    }
}

    // ------------------- Item Database Lookup -------------------

    