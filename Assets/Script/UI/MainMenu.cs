using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [Header("Settings")]
    public int gameSceneIndex = 1; // เลข Scene ด่านแรก

    [Header("UI Panels")]
    public GameObject creditsPanel;

    private void Start()
    {
        // Reset UI
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    // ปุ่ม Start Game
    public void OnStartClick()
    {

        // เรียกผู้จัดการให้พาไป
        if (LoadSceneManager.instance != null)
        {
            LoadSceneManager.instance.LoadScene(gameSceneIndex);
        }
        else
        {
            // Fallback: ถ้าลืมวาง LoadSceneManager ให้โหลดแบบดิบๆ ไปก่อน
            Debug.LogWarning("LoadSceneManager not found! Loading directly.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneIndex);
        }
    }

    // ปุ่ม Credits
    public void ToggleCredits(bool show)
    {
        if (creditsPanel != null) creditsPanel.SetActive(show);
    }

    // ปุ่ม Exit
    public void OnExitClick()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}