using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SkiddishCritter : Critter
{
    [SerializeField]
    float wanderTime, wanderRadius, bushSeekRadius;  // time in seconds for CalcFutureTime(), radius of target point

    [SerializeField]
    float wanderWeight, fleeWeight, pathWeight;  // weights for behaviors
    
    // separationWeight, cohesionWeight, alignmentWeight;

    [SerializeField]
    float distance;  // max distance that critter will flee or seek 

    private GameObject closestBush = null;


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

        Vector2 pathForce = Vector2.zero;
        
        if (Vector2.Distance(transform.position, Player.main.transform.position) < playerRadius)
        {
            pathForce = SeekOnPath() * pathWeight;
        }

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

        if (closestBush != null && !closestBush.IsDestroyed() && Vector2.Distance(transform.position, closestBush.transform.position) < 0.5) {
            maxSpeed = 0.5f;
            return wanderForce;
        }

        // if the player is within a distance - critter flees, otherwise it wanders
        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            maxSpeed = 10;
            return fleeForce; //wanderForce + separationForce + cohesionForce + alignmentForce;
        }

        maxSpeed = 3;

        
        Vector2 fullForce = wanderForce + pathForce;
        fullForce -= fullForce * 2 * Mathf.Pow(20, -1 * Mathf.Abs(Vector2.Distance(transform.position, Player.main.transform.position) - playerRadius / 2));

        return fullForce;

    }

    // sets the position for the critter to "flee" to
    protected override Vector2 CalculateBehavior()
    {
        GameObject[] bushes = GameObject.FindGameObjectsWithTag("Bush");
        float dist = 9999;
        GameObject closest = null;
        foreach (GameObject bush in bushes) {
            if (Vector2.Distance(transform.position, bush.transform.position) < dist) {
                dist = Vector2.Distance(transform.position, bush.transform.position);
                closest = bush;
            }
        }
        Debug.Log("seeking...");
        if (dist < bushSeekRadius) {
            Debug.Log("bush");
            closestBush = closest;
            return closest.transform.position;
        }
        return PickFleePoint();
        
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

    protected override void PlaySound()
    {
        FindObjectOfType<AudioManager>().Play("BushSound");
    }

}
