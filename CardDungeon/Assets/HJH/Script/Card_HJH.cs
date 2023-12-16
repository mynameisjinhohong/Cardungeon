using System.Collections;
using System.Collections.Generic;
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
    void IBeginDragHandler.OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        defaultPos = transform.position;
        GetComponent<Image>().raycastTarget = false;

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
            }
        }
        else
        {
            transform.position = defaultPos;
            GetComponent<Image>().raycastTarget = true;
        }
    }

    void OnDrag()
    {
        
    }
    #endregion
}
