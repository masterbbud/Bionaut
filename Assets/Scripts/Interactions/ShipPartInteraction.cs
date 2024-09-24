using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPartInteraction : InteractibleObject
{
    public override void Interact()
    {
        Player.main.GetComponent<Player>().GiveObject(new ShipPart(1));
        Destroy(gameObject);
    }
}
