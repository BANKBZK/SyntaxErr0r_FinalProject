using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LocalPortal : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ลาก GameObject (Empty) ที่เป็นจุดหมายปลายทางมาใส่ตรงนี้")]
    public Transform destinationPoint;

    [Tooltip("ต้องการให้หันหน้า Player ไปตามทิศของจุดหมายด้วยไหม?")]
    public bool updateRotation = true;

    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็น Player หรือไม่
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }
    }

    private void TeleportPlayer(GameObject playerObj)
    {
        // 1. เล่นเสียงวาร์ป (ถ้ามี SoundManager)
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX("Portal");
        }

        // 2. ย้ายตำแหน่ง (Teleport)
        // สำหรับ Rigidbody การย้าย Transform ทันทีแบบนี้ได้ผลดีที่สุดสำหรับการวาร์ป
        playerObj.transform.position = destinationPoint.position;

        // 3. หันหน้าตามจุดหมาย (Optional)
        if (updateRotation)
        {
            // หมุนตัว Player ให้หันไปทางเดียวกับลูกศร Z (สีน้ำเงิน) ของ Destination
            playerObj.transform.rotation = destinationPoint.rotation;
        }

        // 4. (Optional) หยุดแรงส่งเดิม เพื่อไม่ให้ตัวละครไถลต่อหลังวาร์ป
        Rigidbody rb = playerObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // ใช้ velocity หรือ linearVelocity ตามเวอร์ชัน Unity ของคุณ
            // เนื่องจากโค้ดคุณใช้ linearVelocity (Unity 6+) ผมจะใส่ไว้ให้
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
            rb.velocity = Vector3.zero;
#endif
        }

        Debug.Log("Teleported to " + destinationPoint.name);
    }

    // ฟังก์ชันช่วยวาดเส้นใน Editor จะได้เห็นว่าวาร์ปไปไหน
    private void OnDrawGizmos()
    {
        if (destinationPoint != null)
        {
            Gizmos.color = Color.cyan;
            // วาดเส้นเชื่อมจากประตูไปหาจุดหมาย
            Gizmos.DrawLine(transform.position, destinationPoint.position);
            // วาดวงกลมที่จุดหมาย
            Gizmos.DrawWireSphere(destinationPoint.position, 0.5f);

            // วาดลูกศรบอกทิศทางที่จะหันหน้าไป
            if (updateRotation)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(destinationPoint.position, destinationPoint.forward * 1.5f);
            }
        }
    }
}