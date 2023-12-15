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
        }
    }
    public bool myPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
