using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : Critter
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    float seekWeight;
    
    [SerializeField]
    float followWeight;

    [SerializeField]
    float wanderWeight;

    [SerializeField]
    float wanderRadius;

    [SerializeField]
    float wanderTime;

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

        Vector2 seekForce = Seek(Player.main.transform.position) * seekWeight;

        Vector2 followForce = Follow(Player.main.transform.position) * followWeight;
        
        Vector2 wanderForce = Wander(wanderTime, wanderRadius) * wanderWeight;


        if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        return seekForce + followForce + wanderForce;
    }

    private Vector2 Follow(Vector2 pos)
    {
        if (Vector2.Distance(pos, transform.position) < distance) {
            return -1 * rb.velocity;
        }
        return Seek(pos);
    }

    // Drawing Gizmos function
    //ivate void OnDrawGizmos()
    //
    //  Gizmos.color = Color.yellow;
    //
    //  if (Vector3.Distance(Player.main.transform.position, transform.position) < distance)
    //  {
    //      Gizmos.DrawLine(Player.main.transform.position, transform.position);
    //  }
    //
    //
    //  Gizmos.color = Color.magenta;
    //
    //  Vector3 futurePosition = CalcFuturePosition(wanderTime);
    //  Gizmos.DrawWireSphere(futurePosition, wanderRadius);
    //
    //  Gizmos.color = Color.cyan;
    //  float randAngle = Random.Range(0f, Mathf.PI * 2f);
    //
    //  Vector3 wanderTarget = futurePosition;
    //
    //  wanderTarget.x += Mathf.Cos(randAngle) * wanderRadius;
    //  wanderTarget.y += Mathf.Sin(randAngle) * wanderRadius;
    //
    //  Gizmos.DrawLine(transform.position, wanderTarget);
    //
    //  //Gizmos.color = Color.magenta;
    //  //Gizmos.DrawWireSphere(transform.position, physicsObject.radius);
    //
    //
    //}

}
