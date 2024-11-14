using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CuriousCritter : Critter
{
    [SerializeField]
    float wanderTime, wanderRadius;   // time in seconds for CalcFutureTime(), radius of target point

    [SerializeField]
    float wanderWeight, seekWeight, fleeWeight;  // weights for behaviors

    // separationWeight, cohesionWeight, alignmentWeight;

    [SerializeField]
    float fleeDistance;  // max distance that critter will flee

    [SerializeField]
    float seekDistance;  // max distance that critter will seek


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

        Vector2 seekForce = Seek(Player.main.transform.position) * seekWeight;

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

        // if the player is within a distance - critter flees
        if (Vector2.Distance(Player.main.transform.position, transform.position) < fleeDistance)
        {
            maxSpeed = 10;
            return fleeForce;
            // + wanderForce + separationForce + cohesionForce + alignmentForce;
        }
        // if the player is within a distance and player/critter distance is greater than 4 - critter seeks
        else if (Vector2.Distance(Player.main.transform.position, transform.position) < seekDistance && Vector2.Distance(Player.main.transform.position, transform.position) > 4)
        {
            maxSpeed = 3;
            return -1 * rb.velocity + seekForce;
            // + wanderForce + separationForce + cohesionForce + alignmentForce;
        }
        // otherwise wander
        maxSpeed = 3;
        return wanderForce;
        // + separationForce + cohesionForce + alignmentForce;

    }



    // Gizmos Method
    private void OnDrawGizmos()
    {
       
        if (Vector2.Distance(Player.main.transform.position, transform.position) < fleeDistance)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }
        else if (Vector2.Distance(Player.main.transform.position, transform.position) < seekDistance)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }
    }


}