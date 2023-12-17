using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Credit_HJH : MonoBehaviour
{
    public GameObject credit;
    public GameObject creditTex;
    public float creditSpeed;
    [TextArea]
    public string creditText;

    bool nowCredit = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nowCredit)
        {
            if(Input.GetMouseButtonDown(0))
            {
                credit.SetActive(false);
            }
        }
        
    }
    public void CreditButton()
    {
        StopAllCoroutines();
        nowCredit= true;
        StartCoroutine(CreditCo());
    }

    IEnumerator CreditCo()
    {
        creditTex.GetComponent<TMP_Text>().text = creditText;
        creditTex.GetComponent<RectTransform>().position = new Vector3(0,-1 * creditTex.GetComponent<RectTransform>().sizeDelta.y,0);

        while (true)
        {
            creditTex.GetComponent<RectTransform>().position += new Vector3(0, creditSpeed * Time.deltaTime, 0); 
            yield return null;
            if(creditTex.GetComponent<RectTransform>().position.y > creditTex.GetComponent<RectTransform>().sizeDelta.y)
            {
                credit.SetActive(false);
                break;
            }
        }
    }
}
