using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BushCompanion : Companion
{

    [SerializeField]
    private float killRadius = 5;

    private GameObject closest;

    // this method was created in parent class but each child class has to implement it separately
    protected override Vector2 CalculateSteeringForces()
    {
        if (Vector2.Distance(Player.main.transform.position, transform.position) > 20) {
            transform.position = Player.main.transform.position;
            rb.velocity = Vector2.zero;
            return Vector2.zero;
        }
        min = spriteRenderer.bounds.min;
        max = spriteRenderer.bounds.max;

        Vector2 seekForce = Seek(Player.main.transform.position) * seekWeight;

        Vector2 pathForce = Vector2.zero;
        
        if (Vector2.Distance(transform.position, Player.main.transform.position) < playerRadius)
        {
            pathForce = SeekOnPath() * pathWeight;
        }

        // Flip sprite based on direction
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            spriteRenderer.flipX = rb.velocity.x < 0;
        }

        Debug.Log(closest);
        if (closest != null && !closest.IsDestroyed() && Vector2.Distance(transform.position, closest.transform.position) < killRadius) {
            maxSpeed = 12;
            if (Vector2.Distance(transform.position, closest.transform.position) < 2.5f) {
                Destroy(closest);
                closest = null;
            } else {
                return Seek(closest.transform.position) - rb.velocity;
            }
        }

        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            return -1 * rb.velocity;
        }

        Vector2 fullForce = seekForce + pathForce;
        fullForce -= fullForce * 2 * Mathf.Pow(20, -1 * Mathf.Abs(Vector2.Distance(transform.position, Player.main.transform.position) - playerRadius / 2));

        return fullForce;
    }

    protected override Vector2 CalculateBehavior()
    {
        GameObject[] agressives = GameObject.FindGameObjectsWithTag("Rock");
        float dist = 9999;
        GameObject closest = null;
        foreach (GameObject agg in agressives) {
            if (Vector2.Distance(transform.position, agg.transform.position) < dist) {
                dist = Vector2.Distance(transform.position, agg.transform.position);
                closest = agg;
            }
        }
        if (dist < killRadius) {
            this.closest = closest;
            return closest.transform.position;
        }
        return StopAttackPoint();

    }

}