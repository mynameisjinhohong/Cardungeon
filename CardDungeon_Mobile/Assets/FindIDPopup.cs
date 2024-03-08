using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindIDPopup : MonoBehaviour
{
    [SerializeField]
    TMP_InputField emailInputField;
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && UIManager.Instance.CurrentPopup == gameObject)
        {
            FindAccountWithEmail();
        }
    }
    
    public void FindAccountWithEmail()
    {
        BackendManager.Instance.FindID_WithEmail(emailInputField.text);
    }

    public void ExitPopup()
    {
        UIManager.Instance.PopupListPop();
    }
}
