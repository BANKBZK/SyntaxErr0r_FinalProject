using UnityEngine;
using System.Collections.Generic;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    // --- ส่วนที่ 3: ตัวแปร Array ที่หายไป ---
    [Header("Sound Library")]
    public Sound[] sounds; // <--- บรรทัดนี้จะทำให้เกิดลิสต์ใน Inspector

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

            musicSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlayMusic("BGM");
    }
    // ฟังก์ชันค้นหาเสียงจากชื่อ
    public Sound FindSound(string name)
    {
        // ต้องมี using System; ถึงจะใช้ Array.Find ได้
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
        return s;
    }

    public void PlaySFX(string name)
    {
        Sound s = FindSound(name);
        if (s == null) return;

        sfxSource.pitch = s.pitch;
        sfxSource.PlayOneShot(s.clip, s.volume);
    }

    public void PlayMusic(string name)
    {
        Sound s = FindSound(name);
        if (s == null) return;

        musicSource.clip = s.clip;
        musicSource.volume = s.volume;
        musicSource.pitch = s.pitch;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}