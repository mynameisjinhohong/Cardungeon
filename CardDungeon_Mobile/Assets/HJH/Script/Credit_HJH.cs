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
    public Vector3 firstPos;
    bool nowCredit = false;
    // Start is called before the first frame update
    void Start()
    {
        firstPos = creditTex.GetComponent<RectTransform>().anchoredPosition;
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
        creditTex.GetComponent<RectTransform>().anchoredPosition = firstPos;
        credit.SetActive(true);
        while (true)
        {
            creditTex.GetComponent<RectTransform>().position += new Vector3(0, creditSpeed * Time.deltaTime, 0); 
            yield return null;
            if(creditTex.GetComponent<RectTransform>().anchoredPosition.y > creditTex.GetComponent<RectTransform>().sizeDelta.y)
            {
                credit.SetActive(false);
                break;
            }
        }
    }
}
