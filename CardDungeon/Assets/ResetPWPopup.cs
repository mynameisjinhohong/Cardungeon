using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResetPWPopup : MonoBehaviour
{
    [SerializeField]
    TMP_InputField emailInputField;

    [SerializeField]
    TMP_InputField idInputField;
    
    public void FindAccountWithEmail()
    {
        BackendManager.Instance.ResetPW_WithEmailandID(idInputField.text, emailInputField.text);
    }
    
    public void ExitPopup()
    {
        UIManager.Instance.PopupListPop();
    }
}
