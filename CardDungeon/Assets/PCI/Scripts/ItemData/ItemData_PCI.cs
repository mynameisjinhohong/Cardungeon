using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData_PCI : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public Sprite image;
    public int amount;

    public List<Sprite> sprites = new List<Sprite>();
    public virtual void OnInteracted(Player_HJH player)
    {
        if (!player.isMine)
        {
            Debug.LogError($"player : {player.PlayerName} / myPlayer : {player.isMine}");

            return;
        }
        GamePlayManager.Instance.mainUi.toastMsgContainer.AddMessage(description, 3.0f);
        Debug.Log($"Item Activated : {itemName}({player})");
    }
}
