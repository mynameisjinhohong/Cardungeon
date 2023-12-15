using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using UnityEngine.EventSystems;

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
    }
    void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Vector2 currentPos = eventData.position;
        transform.position = currentPos;
    }
    void IEndDragHandler.OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (transform.position.y > 650)
        {
            playerDeck.UseCard(handIdx);
        }
        else
        {
            transform.position = defaultPos;
        }
    }

    void OnDrag()
    {
        
    }
    #endregion
}
