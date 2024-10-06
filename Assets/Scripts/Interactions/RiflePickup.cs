using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiflePickup : InteractibleObject
{
    public ItemData rifleData;
    public override void Interact()
    {
        // Give the planet a tranquility rifle and delete
        Player.inventory.GiveObject(rifleData, 1);
        Destroy(gameObject);
    }
}
