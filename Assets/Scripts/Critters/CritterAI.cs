using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public abstract class CritterAI : MonoBehaviour, IRifleHittable, INetHittable, IKnifeHittable
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite ActiveSprite;

    [SerializeField]
    Sprite StoppedSprite;

    protected Rigidbody2D rb;

    protected Seeker seeker;  // seeker script

    protected Path path;  // current path

    [SerializeField]
    protected float speed = 5.5f;

    [SerializeField]
    protected float playerRadius = 10.0f;    // radius of the circle around the player where the flee points come from

    [SerializeField]
    protected float nextWaypointDistance = 1.0f;  // how far the nextWaypoint should be

    protected int currentWaypoint = 0;  // current waypoint

    [SerializeField]
    protected float repathRate = 0.5f;

    protected float lastRepath = float.NegativeInfinity;

    protected bool reachedEndOfPath;  // T/F if critter has reached the end of the path

    [SerializeField]
    protected float endDistance = 1.0f;  // distance for critter to stop

    // Instance of the audio manager
    public AudioManager audioManager;
    private bool asleep = false;   // was the critter hit

    public CritterData critterData;

    // While true, the critter doesn't move on its own and can go beyond its
    // max speed.
    private bool freeBody = false;    // ??
    private bool knockedOut = false;   // is the critter knocked out
    private int stamina;   // like health?

    [SerializeField] private int maxStamina = 1;

    private Vector2 externalVelocity;



    // Start Method
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        stamina = maxStamina;
    }


    // Update Method
    void Update()
    {
        if (Vector2.Distance(transform.position, Player.main.transform.position) < playerRadius)
        {
            seekingOnPath();
        }

        if (Vector2.Distance(transform.position, Player.main.transform.position) < playerRadius - 0.5f)
        {
            spriteRenderer.sprite = ActiveSprite;
        }
        else
        {
            spriteRenderer.sprite = StoppedSprite;
        }

        if (externalVelocity != Vector2.zero) {
            transform.position += (Vector3)externalVelocity * Time.deltaTime;
            externalVelocity *= 1 - (1.5f * Time.deltaTime);
            if (externalVelocity.magnitude < 1) {
                externalVelocity = Vector2.zero;
            }
        }
    }



    // implement in each child class - calculates if it is seeking, fleeing, etc
    protected abstract Vector2 CalculateBehavior();


    // method that makes critter seek on the path
    void seekingOnPath()
    {
        if (!knockedOut) {
            // updates lastRepath and calls StartPath()
            if (Time.time > lastRepath + repathRate && seeker.IsDone())
            {
                lastRepath = Time.time;
                seeker.StartPath(transform.position, CalculateBehavior(), OnPathComplete);  // start the path (startPos, endPos, callback)
                                                                                            // CalculateBahavior() sets the position for the critter to seek
            }

            // if there is no valid path, return
            if (path == null)
            {
                return;
            }


            reachedEndOfPath = false;
            float distanceToWaypoint;  // distance to the next waypoint in the path

            // check if agent is close enough to the current waypoint to switch to the next one
            while (true)
            {
                distanceToWaypoint = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);  // calculates distance to next waypoint
                if (distanceToWaypoint < nextWaypointDistance)
                {
                    if (currentWaypoint + 1 < path.vectorPath.Count)  // is there another waypoint or is the agent at the end of the path
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        reachedEndOfPath = true;  // can use this variable to trigger special code if needed
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            // Slow down smoothly (goes from 1 to 0) upon approaching the last waypoint at the end of the path
            float speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;
            if (asleep) {
                speedFactor *= 0.1f;
            }

            Vector3 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;  // Direction to the next waypoint

            Vector3 velocity = direction * speed * speedFactor;  // Multiply the direction by our desired speed to get a velocity

            transform.position += velocity * Time.deltaTime;  // modify the agent position

            
            // flips sprite
            if (Math.Abs(velocity.x) > 0.15f)  // Prevent sprite jitter
            {
                if (velocity.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.flipX = true;
                }
            }
        }
        
        
    }

    // if there is no error for p, set path to p
    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;  // might have to change this to 1 (saw someone else online say that)
        }
    }

    

    // Picks closest point to itself in a certain radius around the player to flee
    public Vector2 PickFleePoint()
    {
        // Get the vector from the GameObject to the circle's center
        Vector2 directionToCenter = (transform.position - Player.main.transform.position).normalized;

        // Calculate the closest point on the circle
        Vector2 closestPoint = (Vector2)Player.main.transform.position + (directionToCenter * playerRadius);

        return closestPoint;
    }



    // radius around player for attack player to stop
    public Vector2 StopAttackPoint()
    {
        // Get the vector from the GameObject to the circle's center
        Vector2 directionToCenter = (transform.position - Player.main.transform.position).normalized;

        // Calculate the closest point on the circle
        Vector2 stopPoint = (Vector2)Player.main.transform.position + (directionToCenter * endDistance);

        return stopPoint;
    }

    //    ------ Player Tools Interacting with Critters ------

    // When the critter is hit with a rifle, call FallAsleep()
    public virtual void OnRifleHit()
    {
        StartCoroutine(FallAsleep());
    }

    // critter falls asleep for 5 seconds
    IEnumerator FallAsleep()
    {
        asleep = true;
        yield return new WaitForSeconds(5);
        asleep = false;
    }

    // When the critter is hit with a net, the player catches it
    public virtual void OnNetHit()
    {
        // Player catches this critter!
        if (Player.inventory.collectedCritters.Contains(critterData))
        {
            // Don't want to catch the same critter twice
            return;
        }
        Player.inventory.AddCritter(critterData);
        CritterManager.DeleteCritter(this);
        Destroy(gameObject);
        CatchCritterDialogBehavior.Show(critterData);
    }

    // When the critter is hit with a knife, call Knockback()
    public virtual void OnKnifeHit()
    {
        if (!knockedOut)
        {
            StartCoroutine(KnockBack());
        }
    }

    // critter gets knocked back and loses stamina
    IEnumerator KnockBack()
    {
        freeBody = true;
        float knockBackAmount = 10f;
        stamina -= 1;
        Debug.Log(stamina);
        if (stamina <= 0)
        {
            // We have to do this bc color is a readonly field
            Color c = spriteRenderer.color;
            c = new Color(0.6f, 0.6f, 0.6f);
            spriteRenderer.color = c;
        }
        ApplyForce((transform.position - Player.main.transform.position).normalized * knockBackAmount);
        yield return new WaitForSeconds(0.5f);

        if (stamina <= 0)
        {
            knockedOut = true;
            rb.drag = 5f;
        }
        freeBody = false;
    }

    private void ApplyForce(Vector2 force) {
        externalVelocity += force;
    }

}
