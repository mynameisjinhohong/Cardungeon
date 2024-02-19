using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player_HJH : MonoBehaviour
{
    //TestCommit
    public int maxHp;
    public int maxMp;
    public float mpCoolTime;
    public TMP_Text PlayerName;
    //mpText용
    public GameObject mpText;
    float mpTextTime;
    Coroutine mpTextCo;
    
    public string PlayerToken;
    public bool isSuperGamer;
    public bool isMine;
    int hp;
    Vector2 myPos;
    bool cool;
    float currentTime;
    bool shield = false;
    public Animator animator;
    public SpriteRenderer sr;
    public int keys;
    public int Keys
    {
        get { return keys; }
        set
        {
            keys = value;
            KeysOnValueChanged?.Invoke(value);
        }
    }
    public Action<int> KeysOnValueChanged;
    public GameObject[] hpSprites;
    public GameObject barrierPrefab;
    public SpriteRenderer playerTile;
    public Sprite shieldTileSprite;
    public Sprite nomalTileSprite;
    Coroutine shieldCo;
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            if (value < hp)
            {
                if (shield)
                {
                    StopCoroutine(shieldCo);
                    shield = false;
                }
                else
                {
                    hp = value;
                    HpRenew(hp);
                    StartCoroutine(GetDamage(1,50));
                    if (isMine)
                    {
                        CameraManager_HJH cam = Camera.main.GetComponent<CameraManager_HJH>();
                        cam.StartCoroutine(cam.Shake(0.3f, 0.3f));
                    }
                }
            }
            else
            {
                hp = value;
            }
            if (isMine)
            {
                GamePlayManager.Instance.mainUi.ReNewHp();
            }
            if (hp > maxHp)
            {
                hp = maxHp;
            }
            if (hp < 1)
            {
                if (isMine)
                {
                    GamePlayManager.Instance.GameOver();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }


    int mp;
    public int Mp
    {
        get
        {
            return mp;
        }
        set
        {
            if(value < 0)
            {
                if(mpTextCo != null)
                {
                    mpTextTime = 0;
                }
                else
                {
                    mpTextCo = StartCoroutine(mpTextOn(1f));
                }
            }
            else
            {
                mp = value;
                if (mp < maxMp)
                {
                    if (!cool)
                    {
                        cool = true;
                        currentTime = 0;
                    }
                }
                else
                {
                    currentTime = 0;
                    cool = false;
                }
                if (isMine)
                {
                    GamePlayManager.Instance.mainUi.ReNewMp();
                }
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        HP = maxHp;
        Mp = maxMp;
    }

    // Update is called once per frame
    void Update()
    {
        if (cool)
        {
            currentTime += Time.deltaTime;
            GamePlayManager.Instance.mainUi.mpCoolTime.fillAmount = currentTime / mpCoolTime;
            if (currentTime / mpCoolTime > 1)
            {
                Mp++;
                currentTime = 0;
            }
        }
        myPos = (Vector2)transform.position;
        if (shield)
        {
            playerTile.sprite = shieldTileSprite;
        }
        else
        {
            playerTile.sprite = nomalTileSprite;
        }
    }
    public void HpRenew(int nowHp)
    {
        for(int i = 0; i < maxHp; i++)
        {
            if(i < nowHp)
            {
                hpSprites[i].SetActive(true);
            }
            else
            {
                hpSprites[i].SetActive(false);
            }
        }
    }


    public void ShieldOn(float time)
    {
        if (shield)
        {
            StopCoroutine(shieldCo);
            shieldCo = StartCoroutine(ShieldCheck(time));
        }
        else
        {
            shieldCo = StartCoroutine(ShieldCheck(time));
        }
    }

    IEnumerator ShieldCheck(float time)
    {
        shield = true;
        barrierPrefab.SetActive(true);
        yield return new WaitForSeconds(time);
        barrierPrefab.SetActive(false);
        shield = false;
    }
    //주석 하나 씀
    IEnumerator GetDamage(float howLongTime, int speed)
    {
        SpriteRenderer playerSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        float time = 0;
        Color color = playerSprite.color;
        bool down = true;
        int co = 255;
        while (true)
        {
            time += 0.05f;
            if (down)
            {
                co -= speed;
                if(co <= 0)
                {
                    co = 0;
                    down = false;
                }
            }
            else
            {
                co += speed;
                if (co >= 255)
                {
                    co = 255;
                    down = true;
                }
            }
            color.g = co / 255f;
            color.b = co / 255f;
            playerSprite.color = color;
            if(time > howLongTime)
            {
                break;
            }
            yield return new WaitForSeconds(0.05f);
        }
        color = Color.white;
        playerSprite.color = color;
    }

    IEnumerator mpTextOn(float time)
    {
        mpText.SetActive(true);
        while(mpTextTime < time)
        {
            mpTextTime += Time.deltaTime;
            yield return null;
        }
        mpTextTime = 0;
        mpText.SetActive(false);
        mpTextCo = null;
    }
}
