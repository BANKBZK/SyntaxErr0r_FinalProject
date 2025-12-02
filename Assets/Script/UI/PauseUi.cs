using UnityEngine;

public class PauseUi : MonoBehaviour
{
    public bool isGamePaused = false;
    private GameObject pauseMenuUI;
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
        Time.timeScale = isGamePaused ? 0 : 1;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(isGamePaused);
    }
}
