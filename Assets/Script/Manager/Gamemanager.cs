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
    // ------------------- Gameplay APIs -------------------

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (hpBar == null) return;
        hpBar.maxValue = maxHealth;
        hpBar.value = currentHealth;
    }
}

    // ------------------- Item Database Lookup -------------------

    