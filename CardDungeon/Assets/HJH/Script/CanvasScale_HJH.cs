using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScale_HJH : MonoBehaviour
{
    CanvasScaler canvasScaler;

    private void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect r = camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / (16f / 9f);
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1f)
        {
            r.height = scaleheight;
            r.y = (1f - scaleheight) / 2f; 
        }
        else 
        {
            r.width = scalewidth;
            r.x = (1f - scalewidth) / 2f; 
        }
        camera.rect = r;
        //canvasScaler = GetComponent<CanvasScaler>();
    }
    private void Start()
    {
         //SetResolution(); 
    }
    public void SetResolution()
    {
        canvasScaler.referenceResolution *= new Vector2((canvasScaler.referenceResolution.x / Screen.width), (Screen.height / canvasScaler.referenceResolution.y));
    }
}

