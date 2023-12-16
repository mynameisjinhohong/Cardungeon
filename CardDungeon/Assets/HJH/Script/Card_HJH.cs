using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card_HJH : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int cardIdx; //� ī������
    public int handIdx; //�ڵ忡�� ���° ī������
    public PlayerDeck_HJH playerDeck;
    public Vector2 defaultPos;
    #region �巡�� �� ���

    void ChildRayCast(bool onOff)
    {
        GetComponent<Image>().raycastTarget = onOff;
        transform.GetChild(0).GetComponent<TMP_Text>().raycastTarget = onOff;
        transform.GetChild(1).GetComponent<TMP_Text>().raycastTarget = onOff;
        transform.GetChild(2).GetComponent<Image>().raycastTarget = onOff;
    }
    void IBeginDragHandler.OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        defaultPos = transform.position;
        ChildRayCast(false);

    }
    void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Vector2 currentPos = eventData.position;
        transform.position = currentPos;
    }
    void IEndDragHandler.OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (!playerDeck.UseCard(handIdx))
            {
                transform.position = defaultPos;
                ChildRayCast(true);
            }
            else
            {
                ChildRayCast(true);
            }
        }
        else
        {
            transform.position = defaultPos;
            GetComponent<Image>().raycastTarget = true;
            ChildRayCast(true);
        }
    }

    public void OnClick()
    {
        ChildRayCast(true);
        playerDeck.mainUi.BigCardOn(cardIdx);
    }
    #endregion
}
