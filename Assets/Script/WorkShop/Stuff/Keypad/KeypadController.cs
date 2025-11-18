using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadController : MonoBehaviour
{
    public Door door;
    [Header("UI / Display")]
    public TextMeshProUGUI displayText; 
    public Image statusImage;  // เปลี่ยนจาก Renderer → Image

    [Header("Settings")]
    public int codeLength = 4;

    private string inputCode = "";
    [SerializeField]private string targetCode = "";
    public AudioClip wrongcheckSFX;
    public AudioClip rightcheckSFX;
    public AudioClip pressSFX;

    private void Start()
    {
        //GenerateRandomCode();
        ResetInput();
    }

    private void LateUpdate()
    {
        CheckKeyboardInput();
    }

    //void GenerateRandomCode()
    //{
    //    targetCode = "";
    //    for (int i = 0; i < codeLength; i++)
    //    {
    //        targetCode += Random.Range(0, 9).ToString();
    //    }
    //     displayCode.text = "Target: " +targetCode;
    //     Debug.Log("Target Code: " + targetCode);
    //} 

    void CheckKeyboardInput()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                AddNumber(i.ToString());
                playOneshotSfx(pressSFX);
            }
        }
    }

    public void AddNumber(string number)
    {
        if (inputCode.Length >= codeLength)
            return;

        inputCode += number;
        displayText.text = inputCode;

        if (inputCode.Length == codeLength)
            ValidateCode();
    }

    void ValidateCode()
    {
        if (inputCode == targetCode) //check if corrects
        {
            playOneshotSfx(rightcheckSFX);
            statusImage.color = Color.green;
            door.isLock = false;
            Destroy(gameObject, 1.0f); // ทำลาย Keypad หลังจากปลดล็อก
        }
        else
        {
            playOneshotSfx(wrongcheckSFX);
            statusImage.color = Color.red;
        }

        Invoke(nameof(ResetInput), 1.2f);
    }

    void ResetInput()
    {
        inputCode = "";
        displayText.text = "----";

        // ตั้งเป็นสีเดิม เช่น ขาว หรือโปร่งใส
        statusImage.color = Color.white;
    }

    public void ClearCode()
    {
        ResetInput();
    }
     void playSfx(AudioClip _sfx)
    {
        GetComponent<AudioSource>().clip = _sfx;
        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
    }
    void playOneshotSfx(AudioClip _sfx)
    {
        GetComponent<AudioSource>().PlayOneShot(_sfx);
    }
}
