using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CuriousCritter : Critter
{
    [SerializeField]
    float wanderTime, wanderRadius;

    //[SerializeField]
    //SpriteRenderer spriteRenderer;

    [SerializeField]
    float wanderWeight, seekWeight, fleeWeight, separationWeight, cohesionWeight, alignmentWeight;

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

        // to make it seek at a certain distance, but flee if player gets too close
        if (Vector2.Distance(Player.main.transform.position, transform.position) < fleeDistance)  // if critter is within fleeDistance
        {
            maxSpeed = 10;
            return wanderForce + fleeForce + separationForce + cohesionForce + alignmentForce;
        }
        else if (Vector2.Distance(Player.main.transform.position, transform.position) < seekDistance && Vector2.Distance(Player.main.transform.position, transform.position) > 4)  // if critter is within seekDistance
        {
            maxSpeed = 3;
            return -1 * rb.velocity + wanderForce + seekForce + separationForce + cohesionForce + alignmentForce;
        }

        maxSpeed = 3;
        return wanderForce + separationForce + cohesionForce + alignmentForce;  // otherwise wander

    }



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