using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AggressiveCritter : Critter
{
    [SerializeField]
    float wanderTime, wanderRadius;

    //[SerializeField]
    //SpriteRenderer spriteRenderer;

    [SerializeField]
    float wanderWeight, seekWeight, separationWeight, cohesionWeight, alignmentWeight;

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

        Vector2 seekForce = Seek(Player.main.transform.position) * seekWeight;

        Vector2 separationForce = Separation(CritterManager.critters) * separationWeight;

        Vector2 cohesionForce = Cohesion(CritterManager.critters) * cohesionWeight;

        Vector2 alignmentForce = Alignment(CritterManager.critters) * alignmentWeight;


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


        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            maxSpeed = 5;
            return -1 * rb.velocity + wanderForce + seekForce + separationForce + cohesionForce + alignmentForce;
        }
        maxSpeed = 3;
        return wanderForce + separationForce + cohesionForce + alignmentForce;

    }


    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }


    }

   



}
