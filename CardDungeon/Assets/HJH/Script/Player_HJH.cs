using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HJH : MonoBehaviour
{
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

    public void getDamage(int damage)
    {
        if(hp- damage > 0)
        {

        }
    }


}
