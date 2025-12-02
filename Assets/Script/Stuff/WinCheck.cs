using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WinCheck : MonoBehaviour
{
    public GameObject winUi;
    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็น Player หรือไม่
        if (other.CompareTag("Player"))
        {
            winUi.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }
}
