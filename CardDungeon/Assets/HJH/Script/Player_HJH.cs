using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HJH : MonoBehaviour
{
    public int maxHp;
    public int maxMp;
    public float mpCoolTime;
    int hp;
    bool cool;
    float currentTime;
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            GamePlayManager.Instance.mainUi.ReNewHp();
            if (hp > maxHp)
            {
                hp = maxHp;
            }
            if(hp < 0)
            {
                if (myPlayer)
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
            if(mp < maxMp)
            {
                if (!cool)
                {
                    cool = true;
                    currentTime = 0;
                }
            }
            else
            {
                currentTime= 0;
                cool = false;
            }
            GamePlayManager.Instance.mainUi.ReNewMp();
        }
    }
    public bool myPlayer;

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
            GamePlayManager.Instance.mainUi.mpCoolTime.fillAmount = currentTime/mpCoolTime;
            if(currentTime / mpCoolTime > 1)
            {
                Mp++;
                currentTime = 0;
            }
        }
    }


}
