using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Key", menuName = "Scriptable Object/Item/Create Key Data")]
public class Key_PCI : ItemData_PCI
{
    [TextArea]
    public string announceAll;
    [TextArea]
    public string announceMe;
    public override void OnInteracted(Player_HJH player)
    {
        player.Keys++;
        base.OnInteracted(player);
        if (player.Keys == 3)
        {
            if (player.isMine)
            {
                GamePlayManager.Instance.mainUi.toastMsgContainer.AddMessage(announceMe, 3.0f);
            }
            else
            {
                GamePlayManager.Instance.mainUi.toastMsgContainer.AddMessage(announceAll, 3.0f);
            }
        }
    }
}
