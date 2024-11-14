using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FleeingBush : CritterAI
{
    //[SerializeField]
    //float minDisToFlee;

    Vector2 fleePoint;   // point for critter to seek to look like fleeing



    // sets the position for the critter to "flee" to
    protected override Vector2 CalculateBehavior()
    {
        fleePoint = PickFleePoint();
        return fleePoint;
        
    }



    // Gizmos Method
    private void OnDrawGizmos()
    {
        if (Vector2.Distance(transform.position, Player.main.transform.position) < playerRadius)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }
            
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Player.main.transform.position, playerRadius);

        Gizmos.color = Color.red;
        Vector2 directionToCenter = (transform.position - Player.main.transform.position).normalized;
        Vector2 closestPoint = (Vector2)Player.main.transform.position + (directionToCenter * playerRadius);
        Gizmos.DrawLine(transform.position, closestPoint);

    }
}
