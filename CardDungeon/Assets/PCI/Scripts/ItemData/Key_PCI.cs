using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Key", menuName = "Scriptable Object/Item/Create Key Data")]
public class Key_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        player.Keys++;
        base.OnInteracted(player);
        if (player.Keys == 3)
        {
            GamePlayManager.Instance.mainUi.toastMsgContainer.AddMessage("누군가가 열쇠를 모두 모았습니다!", 3.0f);
        }
    }
}
