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
    protected override Vector3 CalculateSteeringForces()
    {
        min = spriteRenderer.bounds.min;
        max = spriteRenderer.bounds.max;

        Vector3 wanderForce = Wander(wanderTime, wanderRadius) * wanderWeight;

        Vector3 seekForce = Seek(Player.main.transform.position) * seekWeight;

        Vector3 separationForce = Separation(CritterManager.Instance.critters) * separationWeight;

        Vector3 cohesionForce = Cohesion(CritterManager.Instance.critters) * cohesionWeight;

        Vector3 alignmentForce = Alignment(CritterManager.Instance.critters) * alignmentWeight;

        Vector3 fleeForce = Flee((Player.main.transform.position) * fleeWeight);

        if (physicsObject.Direction.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        if (Vector3.Distance(Player.main.transform.position, transform.position) < distance)
        {
            return wanderForce + seekForce + separationForce + cohesionForce + alignmentForce + fleeForce;
        }

        return wanderForce + separationForce + cohesionForce + alignmentForce;
    }

    // Drawing Gizmos function
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        if (Vector3.Distance(Player.main.transform.position, transform.position) < distance)
        {
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }


        Gizmos.color = Color.magenta;
        
        Vector3 futurePosition = CalcFuturePosition(wanderTime);
        Gizmos.DrawWireSphere(futurePosition, wanderRadius);
        
        Gizmos.color = Color.cyan;
        float randAngle = Random.Range(0f, Mathf.PI * 2f);
        
        Vector3 wanderTarget = futurePosition;
        
        wanderTarget.x += Mathf.Cos(randAngle) * wanderRadius;
        wanderTarget.y += Mathf.Sin(randAngle) * wanderRadius;
        
        Gizmos.DrawLine(transform.position, wanderTarget);

        //Gizmos.color = Color.magenta;
        //Gizmos.DrawWireSphere(transform.position, physicsObject.radius);


    }

}

