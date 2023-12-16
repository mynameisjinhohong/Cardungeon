using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCard_HJH : MonoBehaviour
{

    public int idx;
    public bool imOn = true;
    public void EnforceButton()
    {
        for(int i =0; i< transform.parent.childCount; i++)
        {
            if(i == idx)
            {
                transform.GetChild(4).gameObject.SetActive(true);
                imOn = true;
            }
            else
            {
                transform.parent.GetChild(i).GetChild(4).gameObject.SetActive(false);
                transform.parent.GetChild(i).GetComponent<BigCard_HJH>().imOn = false;
            }
        }
    }
}
