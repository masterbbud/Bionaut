using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite sprite;

    public override int GetHashCode()
    {
        return itemName.GetHashCode();
    }

    public override bool Equals(object other)
    {
        if (other.GetType() != typeof(ItemData)) {
            return false;
        }
        return ((ItemData)other).itemName.Equals(itemName);
    }
}
