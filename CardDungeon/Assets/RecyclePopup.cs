using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RecyclePopup : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI descript;
    public Action action;
    
    public void BackBtnClick()
    {
        UIManager.Instance.PopupListPop();
    }

    public void OkClick()
    {
        action();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OkClick();
                
            UIManager.Instance.PopupListPop();
        }
    }
}
