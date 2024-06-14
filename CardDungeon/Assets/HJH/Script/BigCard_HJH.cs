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
        {
            if(!isDeleteCard) //만약에 카드 강화면
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
            else //만약에 카드 삭제면
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
                    imOn = true;
                    if (isDeleteCard)
                    {
                        transform.GetChild(5).gameObject.SetActive(false);
                        transform.GetChild(6).gameObject.SetActive(true);
                    }
                    else
                    {
                        transform.GetChild(5).gameObject.SetActive(true);
                        transform.GetChild(6).gameObject.SetActive(false);
                    }
                }
                else
                {
                    transform.parent.GetChild(i).GetChild(4).gameObject.SetActive(false);
                    transform.parent.GetChild(i).GetChild(5).gameObject.SetActive(false);
                    transform.parent.GetChild(i).GetChild(6).gameObject.SetActive(false);
                    transform.parent.GetChild(i).GetComponent<BigCard_HJH>().imOn = false;
                }
            }
        }
    }
    private void OnEnable()
    {
        imOn= false;
        transform.GetChild(4).gameObject.SetActive(false);
        transform.GetChild(5).gameObject.SetActive(false);
        transform.GetChild(6).gameObject.SetActive(false);
    }


}
