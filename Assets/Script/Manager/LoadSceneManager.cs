using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadSceneManager : MonoBehaviour
{
    // 1. Singleton Instance
    public static LoadSceneManager instance;

    [Header("UI References")]
    [Tooltip("ลาก Panel ที่เป็นหน้าจอ Loading มาใส่ตรงนี้")]
    public GameObject loadingScreenPanel;

    [Tooltip("ลาก Panel Credits มาใส่ช่องนี้ (ถ้ามี)")]
    public GameObject creditsPanel;

    // 2. Singleton Initialization
    private void Awake()
    {
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

    private void Start()
    {
        // เริ่มมาให้ซ่อนหน้า Credits ก่อนเสมอ
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
        // เล่นเพลงหน้าเมนู (ต้องมี SoundManager และไฟล์เสียงชื่อ 'BGM_Menu')
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayMusic("BGM_Menu");
        }
    }

    // ------------------- Loading System -------------------

    /// <summary>
    /// ใช้กับปุ่ม Start Game: ลากฟังก์ชันนี้ใส่ปุ่ม แล้วกรอกเลข Scene (เช่น 1) ใน Inspector
    /// </summary>
    public void LoadNewScene(int sceneIndex)
    {
        PlayClickSound(); // เล่นเสียงคลิกก่อนโหลด
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(false);

        Debug.Log($"Scene Index '{sceneIndex}' loaded successfully.");
    }

    // ------------------- Credits System -------------------

    public void ToggleCredits(bool show)
    {
        PlayClickSound();
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(show);
        }
    }

    // ------------------- Exit System -------------------

    public void OnExitGameClick()
    {
        PlayClickSound();
        Debug.Log("Quit Game!");

        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    // ------------------- Helper -------------------

    private void PlayClickSound()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX("Click");
        }
    }
}