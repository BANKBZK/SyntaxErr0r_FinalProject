using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // สำหรับ Slider (ถ้ามี)

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager instance;

    [Header("UI Reference")]
    [Tooltip("Panel สีดำ หรือรูป Loading ที่อยู่ใน Canvas ลูกของตัวนี้")]
    public GameObject loadingScreenPanel;

    [Tooltip("หลอดโหลด (Optional: ถ้าไม่มีปล่อยว่างได้)")]
    public Slider progressBar;

    private void Awake()
    {
        // Setup Singleton: ให้มีตัวเดียวและห้ามตาย
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    // ฟังก์ชันนี้ Portal จะเรียกใช้ได้ง่ายๆ
    public void LoadScene(string sceneName)
    {
        // แปลงชื่อเป็น Index (ถ้าจำเป็น) หรือใช้ LoadSceneAsync(string) ก็ได้
        // แต่เพื่อความชัวร์ ใช้ Index ดีกว่าถ้าทำได้
        // ในที่นี้ขอทำแบบรับ string เผื่อ Portal คุณใช้ชื่อฉาก
        StartCoroutine(LoadAsyncString(sceneName));
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        yield return StartCoroutine(ProcessLoading(SceneManager.LoadSceneAsync(sceneIndex)));
    }

    private IEnumerator LoadAsyncString(string sceneName)
    {
        yield return StartCoroutine(ProcessLoading(SceneManager.LoadSceneAsync(sceneName)));
    }

    // Logic การโหลดจริงๆ อยู่ตรงนี้ (ใช้ร่วมกันทั้ง int และ string)
    private IEnumerator ProcessLoading(AsyncOperation operation)
    {
        // 1. เปิดหน้าจอโหลด
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(true);

        // 2. รอจนโหลดเสร็จ
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // อัปเดตหลอดโหลด (ถ้ามี)
            if (progressBar != null) progressBar.value = progress;

            yield return null;
        }

        // 3. ปิดหน้าจอโหลดเมื่อเสร็จ
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(false);
    }
}