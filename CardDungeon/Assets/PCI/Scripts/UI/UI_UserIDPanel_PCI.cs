using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserIDPanel_PCI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI txt_userTitle;
    public TMPro.TextMeshProUGUI txt_userName;
    public TMPro.TextMeshProUGUI txt_userWinRate;
    public Image img_userImage; 
    public GameObject itsmeFrame;

    public bool isEmpty;
    public string TODO_userDataContainer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: 유저 데이터 컨테이너 필요함
    public void SetUserData(string _TODO_userDataContainer)
    {

    }

    public void ClearUserData()
    {

    }
}
