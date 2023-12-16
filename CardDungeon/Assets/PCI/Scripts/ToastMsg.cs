using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToastMsg : MonoBehaviour
{
    public ToastMsgContainer container;
    public TextMeshProUGUI text;
    float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime < 0)
        {
            container.messages.Remove(this);
            Destroy(gameObject);
        }
    }

    public void ShowMessage(string message, float time)
    {
        text.text = message;
        lifeTime = time;
    }
}
