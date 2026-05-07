using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip bgm;
    public AudioClip createCode;

    public static SoundManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static SoundManager instance;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }


    public void SoundPlay(string sound)
    {
        switch (sound)
        {
            case "CreateCode":
                audioSource.PlayOneShot(createCode);
                break;
            default:
                break;
        }
    }
}
