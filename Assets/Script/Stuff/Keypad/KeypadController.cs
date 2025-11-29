using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class KeypadController : Stuff,IInteractable
{
    float resetInputDelay = 1.2f;
    public Door door;
    public GameObject keyPadPanel;
    bool isOpen;
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

    public bool isInteractable { get => isUnlock; set => isUnlock = value; }

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
            door.isUnlock = true;
            StartCoroutine(CloseKeypadAfterDelay(player));


        }
        else
        {
            playOneshotSfx(wrongcheckSFX);
            statusImage.color = Color.red;
        }

        Invoke(nameof(ResetInput), resetInputDelay);
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

    public void Interact(Player player)
    {
        isOpen = !isOpen;
        if(isOpen)
        {
            keyPadPanel.SetActive(true);
            //player.canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            keyPadPanel.SetActive(false);
            //player.canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    IEnumerator CloseKeypadAfterDelay(Player player)
    {
        yield return new WaitForSeconds(resetInputDelay);
        keyPadPanel.SetActive(false);
        Destroy(gameObject);
        //player.canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
