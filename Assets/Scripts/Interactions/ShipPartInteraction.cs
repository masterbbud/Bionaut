using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPartInteraction : InteractibleObject
{
    public ItemData shipPartData;
    public override void Interact()
    {
        Player.inventory.GiveObject(shipPartData, 1);
        Destroy(gameObject);
    }
}
