using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPart : Collectible
{
    public string name = "Ship Part";
    public string description = "A broken part of your ship";
    public int quantity;

    public ShipPart(int quantity) {
        this.quantity = quantity;
    }
}
