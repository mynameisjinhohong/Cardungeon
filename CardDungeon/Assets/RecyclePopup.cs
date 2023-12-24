using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
}
