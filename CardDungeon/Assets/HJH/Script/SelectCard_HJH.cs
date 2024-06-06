using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCard_HJH : MonoBehaviour
{
    public TMP_Text cardName;
    public TMP_Text cardDescribe;
    public TMP_Text cardCost;
    public Image cardImage;
    public Image cardFrame;
    private void OnEnable()
    {
        cardFrame.sprite = transform.parent.GetComponent<Image>().sprite;
        cardCost.text = transform.parent.GetChild(0).GetComponent<TMP_Text>().text;
        cardName.text = transform.parent.GetChild(1).GetComponent<TMP_Text>().text;
        cardDescribe.text = transform.parent.GetChild(2).GetComponent<TMP_Text>().text;
        cardImage.sprite = transform.parent.GetChild(3).GetComponent<Image>().sprite;
    }
}
