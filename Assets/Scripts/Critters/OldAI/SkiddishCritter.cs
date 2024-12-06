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
        Vector2 wanderForce = Wander(wanderTime, wanderRadius) * wanderWeight;
        Vector2 fleeForce = Flee(Player.main.transform.position) * fleeWeight;

        if (closestBush != null && !closestBush.IsDestroyed() &&
            Vector2.Distance(transform.position, closestBush.transform.position) < 0.5f)
        {
            maxSpeed = 0.5f;
            animator.SetBool("IsFleeing", true);
            return wanderForce;
        }

        // Flee behavior
        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            maxSpeed = 10;
            animator.SetBool("IsFleeing", true);

            // Flip sprite to face away from the player
            Vector2 fleeDirection = (transform.position - Player.main.transform.position).normalized;
            spriteRenderer.flipX = fleeDirection.x < 0; // Corrected flipping logic

            return fleeForce;
        }

        // Default wander behavior
        maxSpeed = 3;
        animator.SetBool("IsFleeing", true);

        // Flip sprite based on movement direction
        if (rb.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = false; // Moving right
        }
        else if (rb.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = true; // Moving left
        }

        return wanderForce;
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
            animator.SetBool("IsFleeing", true);
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
