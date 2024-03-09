using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScale_HJH : MonoBehaviour
{
    CanvasScaler canvasScaler;

    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }
    private void Start()
    {
        SetResolution(); // �ʱ⿡ ���� �ػ� ����
    }

    /* �ػ� �����ϴ� �Լ� */
    public void SetResolution()
    {
        Debug.Log(canvasScaler.referenceResolution);
        canvasScaler.referenceResolution *= new Vector2((canvasScaler.referenceResolution.x / Screen.width), (Screen.height / canvasScaler.referenceResolution.y));
    }
}
