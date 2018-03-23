using System;
using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public bool use = false;
    public AudioSource audioSource;

    private string nextMusicName = "";
    private bool loading = false;
    private bool loop = false;

    public void Init()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play(string musicName, bool loop_ = false , bool instant = true)
    {
        if (musicName == "") return;
        loop = loop_;
        nextMusicName = musicName;
        if (instant)
        {
            loading = true;
            LoadAsset();
        }
    }

    void Update()
    {
        if (!loading && !audioSource.isPlaying)
        {
            if (nextMusicName != "")
            {
                if (!loading)
                {
                    loading = true;
                    LoadAsset();
                }
            }
        }
    }

    void LoadAsset()
    {
        audioSource.loop = loop;
        ResourceManager.Instance.LoadAsset("resourceassets/SoundAssets.assetbundle", ab =>
        {
            AudioClip clip = ab.LoadAsset<AudioClip>(nextMusicName + ".mp3");
            audioSource.clip = clip;
            audioSource.Play();
            nextMusicName = "";
            loading = false;
        });
    }
}

