using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data List", menuName = "Scriptable Object/Item/Create Item Data List")]
public class ItemDataList_PCI : ScriptableObject
{
    public List<ItemData_PCI> itemDataList = new List<ItemData_PCI>();
}
