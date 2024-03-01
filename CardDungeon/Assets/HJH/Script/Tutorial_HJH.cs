using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial_HJH : MonoBehaviour
{
    public GameObject[] pages;
    public TMP_Text pageText;
    int idx = 0;
    public int Idx
    {
        get
        {
            return idx;
        }
        set
        {
            idx = value;
            for(int i =0; i < pages.Length; i++)
            {
                if(i == idx)
                {
                    pages[i].gameObject.SetActive(true);
                }
                else
                {
                    pages[i].gameObject.SetActive(false);
                }
            }
            pageText.text = (idx + 1) + " / " + pages.Length;
        }
    }
    // Start is called before the first frame update
    public void OnEnable()
    {
        idx = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
