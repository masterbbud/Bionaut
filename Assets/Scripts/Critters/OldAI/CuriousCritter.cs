using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CuriousCritter : Critter
{
    [SerializeField]
    float wanderTime, wanderRadius;   // time in seconds for CalcFutureTime(), radius of target point

    [SerializeField]
    float wanderWeight, seekWeight, fleeWeight, pathWeight;  // weights for behaviors

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

        // if the player is within a distance - critter flees
        // if (Vector2.Distance(Player.main.transform.position, transform.position) < fleeDistance)
        // {
        //     maxSpeed = 10;
        //     return fleeForce;
        //     // + wanderForce + separationForce + cohesionForce + alignmentForce;
        // }
        // // if the player is within a distance and player/critter distance is greater than 4 - critter seeks
        // else if (Vector2.Distance(Player.main.transform.position, transform.position) < seekDistance && Vector2.Distance(Player.main.transform.position, transform.position) > 4)
        // {
        //     maxSpeed = 3;
        //     return -1 * rb.velocity + seekForce;
        //     // + wanderForce + separationForce + cohesionForce + alignmentForce;
        // }
        // otherwise wander
        maxSpeed = 3;

        
        Vector2 fullForce = wanderForce + pathForce;
        fullForce -= fullForce * 2 * Mathf.Pow(20, -1 * Mathf.Abs(Vector2.Distance(transform.position, Player.main.transform.position) - playerRadius / 2));

        return fullForce;
        // + separationForce + cohesionForce + alignmentForce;

    }

    // sets the position for the critter to "flee" to
    protected override Vector2 CalculateBehavior()
    {
        // if (stayInStateTime > 0) {
        //     stayInStateTime -= Time.deltaTime;
        //     if (lastBehavior == BehaviourEnum.FLEE) {
        //         return PickFleePoint();
        //     }
        //     else if (lastBehavior == BehaviourEnum.ATTACK) {
        //         return StopAttackPoint();
        //     }
        // }
        // stayInStateTime = 0.5f;
        // Debug.Log("reevaluating");
        if (Vector2.Distance(transform.position, Player.main.transform.position) < playerRadius / 2)
        {
            if (lastBehavior != BehaviourEnum.FLEE) {
                rb.velocity /= 4;
            }
            lastBehavior = BehaviourEnum.FLEE;
            return PickFleePoint();
        }
        else
        {
            if (lastBehavior != BehaviourEnum.ATTACK) {
                rb.velocity /= 4;
            }
            lastBehavior = BehaviourEnum.ATTACK;
            return StopAttackPoint();
        }
        
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

    protected override void PlaySound()
    {
        FindObjectOfType<AudioManager>().Play("GlorpSound");
    }
}