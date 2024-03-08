using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class AudioPlayer : Singleton<AudioPlayer>
{
    private static AudioPlayer instance;
    private float timer = 300;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(timer > 180)
        {
            return;
        }
        timer += Time.deltaTime;

        if (timer > 180)
        {
            PlayBGM(2);
        }
    }

    // 이벤트 리스너 등록
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 이벤트 리스너 제거
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0)
        {
            timer = 300;
            PlayBGM(0);
        }
        else
        {
            PlayBGM(1);
            timer = 0;
        }
    }

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
    /// 11. box_open
    /// 12 . cardd_stop
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
        bgmPlayer.Stop();
        bgmPlayer.clip = bgmClips[idx];
        bgmPlayer.Play();
    }
}
