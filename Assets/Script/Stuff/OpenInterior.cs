using UnityEditor.Analytics;
using UnityEngine;

public class OpenInterior : MonoBehaviour
{
    public GameObject roof;
    Collider _col;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent<Player>(out var player)) return;
        roof.SetActive(false);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent<Player>(out var player)) return;
        roof.SetActive(true);
    }
}
