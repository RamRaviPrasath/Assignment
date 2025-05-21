using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource BgmSource;
    public AudioSource audioSource;
    public AudioClip Bgm;
    public AudioClip UIClick;
    public AudioClip CoinSound;

    private void Start()
    {
        instance = this;
        PlayBGM();
    }

    public void PlayBGM()
    {
        BgmSource.clip = Bgm;
        BgmSource.loop = true;
        BgmSource.Play();
    }

    public void PlaySound(string type)
    {
        audioSource.Stop();
        audioSource.clip = type.ToLower() == "coin" ? CoinSound : UIClick;
        audioSource.Play();
    }
}
