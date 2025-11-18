using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// กำหนดให้เป็น sealed เพื่อป้องกันการสืบทอด
public class GameManager : MonoBehaviour
{
    public List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();
    // 1. Private Static Field (The Singleton Instance)
    // ใช้ backing field เพื่อควบคุมการเข้าถึง
    public static GameManager instance;
    // 2. Public Static Property (Global Access Point)
   
    [Header("Game State")]
    public int currentScore = 0;
    public bool isGamePaused = false;

    [Header("UI Game")]
    public GameObject pauseMenuUI;
    public AudioClip pauseSound; //Here
    public TMP_Text scoreText;
    public Slider HPBar;

    // 3. Private Constructor Logic (ใช้ Awake() แทน Constructor ปกติใน Unity)
    private void Awake()
    {
        // ตรวจสอบว่ามี Instance อยู่แล้วหรือไม่
       if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
       else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // ------------------- Singleton Functionality -------------------

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
       HPBar.value = currentHealth;
        HPBar.maxValue = maxHealth;
    }
    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreText.text = currentScore.ToString();
    }

    public void TogglePause()
    {
       isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0 : 1;
        pauseMenuUI.SetActive(isGamePaused);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            SoundManager.instance.PlaySFX(pauseSound); //Here
        }
    }
    public ItemDefinition FindById(string id)
    {
        return itemDefinitions.Find(d => d.Id == id);
    }

}