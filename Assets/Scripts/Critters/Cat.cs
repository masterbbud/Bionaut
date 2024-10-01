using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Critter
{
    [SerializeField]
    float wanderTime, wanderRadius;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    float wanderWeight, seekWeight, separationWeight, cohesionWeight, alignmentWeight, fleeWeight;

    [SerializeField]
    float distance;  // max distance that critter will flee or seek 

    void Start()
    {
        min = spriteRenderer.bounds.min;
        max = spriteRenderer.bounds.max;
    }

    // this method was created in parent class but each child class has to implement it separately
    protected override Vector2 CalculateSteeringForces()
    {
        min = spriteRenderer.bounds.min;
        max = spriteRenderer.bounds.max;

        Vector2 wanderForce = Wander(wanderTime, wanderRadius) * wanderWeight;

        Vector2 seekForce = Seek(Player.main.transform.position) * seekWeight;

        Vector2 separationForce = Separation(CritterManager.critters) * separationWeight;

        Vector2 cohesionForce = Cohesion(CritterManager.critters) * cohesionWeight;

        Vector2 alignmentForce = Alignment(CritterManager.critters) * alignmentWeight;

        Vector2 fleeForce = Flee(Player.main.transform.position) * fleeWeight;

        if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            return wanderForce + seekForce + separationForce + cohesionForce + alignmentForce + fleeForce;
        }

        return wanderForce + separationForce + cohesionForce + alignmentForce;
    }

    // Drawing Gizmos function
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }


        Gizmos.color = Color.magenta;
        
        Vector2 futurePosition = CalcFuturePosition(wanderTime);
        Gizmos.DrawWireSphere(futurePosition, wanderRadius);
        
        Gizmos.color = Color.cyan;
        float randAngle = Random.Range(0f, Mathf.PI * 2f);
        
        Vector2 wanderTarget = futurePosition;
        
        wanderTarget.x += Mathf.Cos(randAngle) * wanderRadius;
        wanderTarget.y += Mathf.Sin(randAngle) * wanderRadius;
        
        Gizmos.DrawLine(transform.position, wanderTarget);

        //Gizmos.color = Color.magenta;
        //Gizmos.DrawWireSphere(transform.position, physicsObject.radius);


    }

}

