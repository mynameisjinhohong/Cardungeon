using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HJH : MonoBehaviour
{
    //TestCommit
    public int maxHp;
    public int maxMp;
    public float mpCoolTime;
    public string PlayerName;
    public string PlayerToken;
    public bool isSuperGamer;
    public bool isMine;
    int hp;
    bool cool;
    float currentTime;
    bool shield = false;
    public Animator animator;

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
            if (hp < 0)
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
        yield return new WaitForSeconds(time);
        shield = false;
    }
    //주석 하나 씀
}
