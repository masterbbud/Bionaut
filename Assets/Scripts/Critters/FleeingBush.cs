using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FleeingBush : CritterAI
{
    [SerializeField]
    Sprite FleeSprite;

    [SerializeField]
    Sprite StoppedSprite;

    [SerializeField]
    float minDisToFlee;  // min distance for critter to flee

    Vector2 fleePoint = Vector2.zero;   // point for critter to seek to look like fleeing



    // sets the position for the critter to "flee" to
    protected override Vector2 CalculateBehavior()
    {
        //spriteRenderer.sprite = StoppedSprite;

        // if distance is greater than minDisToFlee, pick a flee point
        if (Vector2.Distance(transform.position, Player.main.transform.position) < minDisToFlee)
        {
            fleePoint = PickFleePoint();
        }

        return fleePoint;
    }



    // Gizmos Method
    private void OnDrawGizmos()
    {
        if (Vector2.Distance(transform.position, Player.main.transform.position) < minDisToFlee)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }
            
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Player.main.transform.position, fleePointPlayerRadius);

        Gizmos.color = Color.red;
        Vector2 directionToCenter = (transform.position - Player.main.transform.position).normalized;
        Vector2 closestPoint = (Vector2)Player.main.transform.position + (directionToCenter * fleePointPlayerRadius);
        Gizmos.DrawLine(transform.position, closestPoint);

    }
}
