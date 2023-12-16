using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Upgrade Item", menuName = "Scriptable Object/Item/Create Card Upgrade Item")]
public class ItemEnfofrce_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        GamePlayManager.Instance.mainUi.EnforceOn();
        base.OnInteracted(player);
    }
}
