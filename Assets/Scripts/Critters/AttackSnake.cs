using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackSnake : CritterAI
{
    Vector2 stopPoint;   // point for critter to seek/stop around player

    // sets seek point to Player
    protected override Vector2 CalculateBehavior()
    {
        //AttackCritterHit();
        stopPoint = StopAttackPoint();
        return stopPoint;

    }


    // Gizmos Method
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Player.main.transform.position, endDistance);

        //Gizmos.color = Color.red;
        //Vector2 directionToCenter = (transform.position - Player.main.transform.position).normalized;
        //Vector2 closestPoint = (Vector2)Player.main.transform.position + (directionToCenter * endDistance);
        //Gizmos.DrawLine(transform.position, closestPoint);
    }

    private void Update()
    {
        if (UnityEngine.Random.Range(0f, 10f) < 2f)
        {
            Debug.Log("Running");
            FindObjectOfType<AudioManager>().Play("LaserSound");
        }
    }
}
