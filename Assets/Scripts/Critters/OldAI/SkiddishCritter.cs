using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkiddishCritter : Critter
{
    [SerializeField]
    float wanderTime, wanderRadius;  // time in seconds for CalcFutureTime(), radius of target point

    [SerializeField]
    float wanderWeight, fleeWeight;  // weights for behaviors
    
    // separationWeight, cohesionWeight, alignmentWeight;

    [SerializeField]
    float distance;  // max distance that critter will flee or seek 


    protected override void StartSubclass()
    {
        min = spriteRenderer.bounds.min;
        max = spriteRenderer.bounds.max;
    }


    protected override Vector2 CalculateSteeringForces()
    {
        min = spriteRenderer.bounds.min;
        max = spriteRenderer.bounds.max;

        Vector2 wanderForce = Wander(wanderTime, wanderRadius) * wanderWeight;

        Vector2 fleeForce = Flee(Player.main.transform.position) * fleeWeight;

        //Vector2 separationForce = Separation(CritterManager.critters) * separationWeight;

        //Vector2 cohesionForce = Cohesion(CritterManager.critters) * cohesionWeight;

        //Vector2 alignmentForce = Alignment(CritterManager.critters) * alignmentWeight;


        // flips sprite based on direction critter is going
        if (Math.Abs(rb.velocity.x) > 0.1f)  // Prevent sprite jitter
        {
            if (rb.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }


        // if the player is within a distance - critter flees, otherwise it wanders
        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            maxSpeed = 10;
            return fleeForce; // wanderForce + separationForce + cohesionForce + alignmentForce;
        }
        maxSpeed = 4;
        return wanderForce; // + separationForce + cohesionForce + alignmentForce;

    }


    // Gizmos Method
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;

    //     if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
    //     {
    //         Gizmos.DrawLine(Player.main.transform.position, transform.position);
    //     }
    // }


}
