using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private int loadScene; // พิมพ์ชื่อฉากใน Inspector ได้เลย

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // สั่งโหลดฉากเมื่อผู้เล่นเดินชน
            LoadSceneManager.instance.LoadNewScene(loadScene);
        }
    }
}