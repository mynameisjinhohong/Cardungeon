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
    public GameObject[] hpSprites;
    public GameObject barrierPrefab;
    public SpriteRenderer playerTile;
    public Sprite shieldTileSprite;
    public Sprite nomalTileSprite;
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
                    StopAllCoroutines();
                    shield = false;
                }
                else
                {
                    hp = value;
                    HpRenew(hp);
                    if (isMine)
                    {
                        CameraManager_HJH cam = Camera.main.GetComponent<CameraManager_HJH>();
                        cam.StartCoroutine(cam.Shake(0.5f, 0.5f));
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
            StopAllCoroutines();
            StartCoroutine(ShieldCheck(time));
        }
        else
        {
            StartCoroutine(ShieldCheck(time));
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
}
