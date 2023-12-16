using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastMsgContainer : MonoBehaviour
{
    public ToastMsg toastMsgPrefab;

    public List<ToastMsg> messages = new List<ToastMsg>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMessage(string message, float time)
    {
        foreach(var e in messages)
        {
            e.transform.Translate(200 * Vector3.up);
        }
        var newToastMsg = Instantiate(toastMsgPrefab, transform);
        newToastMsg.ShowMessage(message, time);
        newToastMsg.container = this;
        messages.Add(newToastMsg);
    }
}
