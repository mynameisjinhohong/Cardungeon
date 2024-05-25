using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCard_HJH : MonoBehaviour
{
    public MainUI_HJH mainUi;
    public int idx;
    public bool imOn = false;
    public bool isDeleteCard;
    
    public void EnforceButton()
    {
        if (imOn)
        {   if(!isDeleteCard)
            {
                if (mainUi != null)
                {
                    mainUi.EnforceEnd();
                }
                else
                {
                    mainUi = GamePlayManager.Instance.mainUi;
                    mainUi.EnforceEnd();
                }
            }
            else
            {
                if (mainUi != null)
                {
                    mainUi.DeleteEnd();
                }
                else
                {
                    mainUi = GamePlayManager.Instance.mainUi;
                    mainUi.DeleteEnd();
                }
            }

        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (transform.parent.GetChild(i) == transform)
                {
                    transform.GetChild(4).gameObject.SetActive(true);
                    transform.GetChild(5).gameObject.SetActive(true);
                    imOn = true;
                }
                else
                {
                    transform.parent.GetChild(i).GetChild(4).gameObject.SetActive(false);
                    transform.parent.GetChild(i).GetChild(5).gameObject.SetActive(false);
                    transform.parent.GetChild(i).GetComponent<BigCard_HJH>().imOn = false;
                }
            }
        }
    }
    
    
}
