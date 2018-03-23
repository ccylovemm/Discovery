using System;
using UnityEngine;
using System.Collections.Generic;

public class SoundManager : Singleton<SoundManager>
{
    private List<MusicPlayer> musicLists = new List<MusicPlayer>();

    public MusicPlayer GetMuiscPlayer()
    {
        foreach (MusicPlayer music in this.musicLists)
        {
            if (!music.use && !music.audioSource.isPlaying)
            {
                return music;
            }
        }
        MusicPlayer musicPlayer = new GameObject("AudioSource" + (musicLists.Count + 1)).AddComponent<MusicPlayer>();
        musicPlayer.transform.parent = transform;
        musicPlayer.Init();
        musicPlayer.use = true;
        musicLists.Add(musicPlayer);
        return musicPlayer;
    }
}

