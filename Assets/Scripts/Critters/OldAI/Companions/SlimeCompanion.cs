using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SlimeCompanion : Companion
{

    // this method was created in parent class but each child class has to implement it separately
    protected override Vector2 CalculateSteeringForces()
    {
        min = spriteRenderer.bounds.min;
        max = spriteRenderer.bounds.max;

        Vector2 seekForce = Seek(Player.main.transform.position) * seekWeight;


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
            return -1 * rb.velocity;
        }

        return seekForce;
    }

    protected override Vector2 CalculateBehavior()
    {
        return StopAttackPoint();

    }

}