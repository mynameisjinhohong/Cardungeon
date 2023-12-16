using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card_HJH : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int cardIdx; //어떤 카드인지
    public int handIdx; //핸드에서 몇번째 카드인지
    public PlayerDeck_HJH playerDeck;
    public Vector2 defaultPos;
    #region 드래그 앤 드롭
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

    public void OnClick()
    {
        playerDeck.mainUi.BigCardOn(cardIdx);
    }
    #endregion
}
