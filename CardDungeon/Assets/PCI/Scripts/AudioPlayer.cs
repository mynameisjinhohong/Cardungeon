using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : Singleton<AudioPlayer>
{
    public List<AudioClip> sfxClips = new List< AudioClip>();
    public List<AudioClip> bgmClips = new List<AudioClip>();
    public AudioSource sfxPlayer, bgmPlayer;

    /// <summary>
    /// 0. btn_close
    /// 1. btn_open
    /// 2. card_info
    /// 3. card_use
    /// 4. carrot_attack1
    /// 5. carrot_attack2
    /// 6. carrot_attack3
    /// 7. rabbit_fall
    /// 8. rabbit_walk1
    /// 9. rabbit_walk2
    /// 10. rabbit_walk3
    /// </summary>
    /// <param name="idx"></param>
    public void PlayClip(int idx)
    {
        sfxPlayer.clip = sfxClips[idx];
        sfxPlayer.Play();
    }

    /// <summary>
    /// 0. cardd_bgm00
    /// 1. cardd_bgm01
    /// 2. cardd_bgm02
    /// </summary>
    /// <param name="idx"></param>
    public void PlayBGM(int idx)
    {
        bgmPlayer.clip = bgmClips[idx];
        bgmPlayer.Play();
    }
}
