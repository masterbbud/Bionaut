using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnakeCompanion : Companion
{

    public override void OnSelect()
    {
        Player.main.GetComponent<Player>().isSpedUp = true;
    }

    public override void OnDeselect()
    {
        Player.main.GetComponent<Player>().isSpedUp = false;
    }

}