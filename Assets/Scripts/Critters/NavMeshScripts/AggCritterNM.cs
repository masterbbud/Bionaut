using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class AggCritterNM : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected Rigidbody2D rb;

    [SerializeField]
    protected float distance;   //for seeking player

    NavMeshAgent agent;


    float randAngle;

    public float wanderRadius;    // for wandering
    public float wanderTimer;

    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        timer = wanderTimer;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            agent.speed = 5.0f;
            agent.SetDestination(Player.main.transform.position);
        }
        else
        {
            agent.speed = 2.0f;

            if (timer >= wanderTimer)
            {
                agent.SetDestination(WanderPoint());
                timer = 0;
            }
        }


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


    }


    public Vector2 WanderPoint()
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * wanderRadius;

        randDirection += transform.position;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, wanderRadius, -1);

        return navHit.position;
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
