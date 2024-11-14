using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CuriousBlob : CritterAI
{
    Vector2 fleePoint;   // point for critter to seek to look like fleeing

    Vector2 stopPoint;   // point for critter to seek/stop around player


    // sets the position for the critter to "flee" to
    protected override Vector2 CalculateBehavior()
    {
        if (Vector2.Distance(transform.position, Player.main.transform.position) < playerRadius / 2)
        {
            fleePoint = PickFleePoint();
            return fleePoint;
        }
        else
        {
            stopPoint = StopAttackPoint();
            return stopPoint;
        }
        
    }



    // Gizmos Method
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Player.main.transform.position, playerRadius);

        Gizmos.color = Color.red;
        Vector2 directionToCenter = (transform.position - Player.main.transform.position).normalized;
        Vector2 closestPoint = (Vector2)Player.main.transform.position + (directionToCenter * playerRadius);
        Gizmos.DrawLine(transform.position, closestPoint);

        Gizmos.color = Color.red;
        Vector2 directionToCenter1 = (transform.position - Player.main.transform.position).normalized;
        Vector2 closestPoint1 = (Vector2)Player.main.transform.position + (directionToCenter1 * playerRadius / 2);
        Gizmos.DrawLine(transform.position, closestPoint1);

    }
}