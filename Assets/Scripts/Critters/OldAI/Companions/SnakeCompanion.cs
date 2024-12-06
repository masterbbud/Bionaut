using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCompanion : Companion
{
    public override void OnSelect()
    {
        // Enable speed-up effect
        Player.main.GetComponent<Player>().isSpedUp = true;

        // Flip the companion on the x-axis
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * -1; // Ensure flipping is consistent
        transform.localScale = scale;
    }

    public override void OnDeselect()
    {
        // Disable speed-up effect
        Player.main.GetComponent<Player>().isSpedUp = false;

        // Flip the companion back on the x-axis
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x); // Ensure it returns to the original orientation
        transform.localScale = scale;
    }
}
