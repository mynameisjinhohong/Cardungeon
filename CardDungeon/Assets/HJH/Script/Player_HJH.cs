using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HJH : MonoBehaviour
{
    public int maxHp;
    public int maxMp;
    int hp;
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
        
    }


}
