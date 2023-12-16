using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Key", menuName = "Scriptable Object/Item/Create Key Data")]
public class Key_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        //TODO: 열쇠 얻는 기능 구현하기
    }
}
