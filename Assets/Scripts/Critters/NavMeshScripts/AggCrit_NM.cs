using System;
using UnityEngine;
using UnityEngine.AI;

public class AggCrit_NM : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected Rigidbody2D rb;

    [SerializeField]
    protected float distance;   //for seeking player

    NavMeshAgent agent;

    [SerializeField]
    float stoppingDis;

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


        if (Math.Abs(agent.velocity.x) > 0.1f)  // Prevent sprite jitter
        {
            if (agent.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }



        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            agent.speed = 3.5f;
            agent.stoppingDistance = stoppingDis;
            agent.SetDestination(Player.main.transform.position);
            spriteRenderer.color = new Color32(219, 118, 118, 255);  // red-ish

        }
        else
        {
            agent.speed = 2.0f;

            if (timer >= wanderTimer)
            {
                agent.stoppingDistance = 0;
                agent.SetDestination(WanderPoint());
                timer = 0;
            }
            spriteRenderer.color = new Color32(255, 255, 255, 255);  // white-ish
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