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
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (emailInputField.isFocused)
            {
                idInputField.Select();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Tab))
        {
            if (idInputField.isFocused)
            {
                emailInputField.Select();
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Return) && UIManager.Instance.CurrentPopup == gameObject)
        {
            FindAccountWithEmailAndID();
        }
    }
    
    public void FindAccountWithEmailAndID()
    {
        BackendManager.Instance.ResetPW_WithEmailandID(idInputField.text, emailInputField.text);
    }
    
    public void ExitPopup()
    {
        UIManager.Instance.PopupListPop();
    }
}
