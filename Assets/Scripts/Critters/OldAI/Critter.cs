using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;


public abstract class Critter : MonoBehaviour, IRifleHittable, INetHittable, IKnifeHittable
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected Rigidbody2D rb;

    protected Vector2 totalForces = Vector2.zero;

    [SerializeField]
    float maxForce = 5f;
    

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

    [SerializeField]
    float maxDisBetweenCritters = 1.0f;  // used for separation, cohesion, and alignment

    float randWanderAngle;  // used in wander

    protected Vector2 min, max;  // used with with the sprite renderer in child classes

    private bool asleep = false;   // was the critter hit

    public CritterData critterData;

    // While true, the critter doesn't move on its own and can go beyond its
    // max speed.
    private bool freeBody = false;    // ??
    public bool knockedOut = false;   // is the critter knocked out
    //private int stamina;   // like health?
    //private int maxStamina;   // like health?


    [SerializeField]
    private int maxHealth = 30;  // starting health variable

    [SerializeField]
    private int health;  // changing health variable



    [SerializeField]
    float mass = 1f;  // mass of critter; used in ApplyForces()

    [SerializeField]
    protected float maxSpeed;  // maxSpeed of critter

    // Instance of the audio manager
    public AudioManager audioManager;

    protected BehaviourEnum lastBehavior = BehaviourEnum.FLEE;
    protected float stayInStateTime = 0.0f;

    public Animator animator;
    public Vector3 facingDirection = Vector3.zero;



    /*
          ----------- Methods -----------
    */


    // Start Method
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        //stamina = maxStamina;
        health = maxHealth;
        randWanderAngle = UnityEngine.Random.Range(0f, 360f);
        StartSubclass();
    }

    // Outlined within the childclasses
    protected virtual void StartSubclass() {}

    //Sound Queue to be overwritten
    protected virtual void PlaySound() {}

    // Update Method
    void Update()
    {
        if (!freeBody && !knockedOut) {
            totalForces += CalculateSteeringForces();

            totalForces = Vector2.ClampMagnitude(totalForces, maxForce);

            ApplyForce(totalForces);
        }
        
        totalForces = Vector2.zero;

        if (UnityEngine.Random.Range(0f, 10f) < 0.002f)
        {
            PlaySound();
        }

    }








    // abstract method = you implement the function in each child class
    protected abstract Vector2 CalculateSteeringForces();


    // calculates the future position of agent
    public Vector2 CalcFuturePosition(float futureTime)
    {
        return (Vector2)transform.position + (rb.velocity * futureTime);  // returns a world point because of the transform.position
    }

    // Apply forces to critter
    public void ApplyForce(Vector2 force)
    {
        rb.velocity += (force / mass) * Time.deltaTime;   // += REALLY IMPORTANT!
        float currentMaxSpeed = maxSpeed;
        if (asleep)
        {
            currentMaxSpeed *= 0.02f;
        }
        if (!freeBody)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, currentMaxSpeed);
        }
    }




    /*   
         ----------- Critter Behaviors -----------
    */

    // method that makes critter seek on the path
    protected Vector2 SeekOnPath()
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
                return Vector2.zero;
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

            float maxSpeed = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

            Vector3 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;  // Direction to the next waypoint

            Vector3 velocity = direction * speed * speedFactor;  // Multiply the direction by our desired speed to get a velocity

            return velocity;
            
            // // flips sprite
            // if (Math.Abs(velocity.x) > 0.15f)  // Prevent sprite jitter
            // {
            //     if (velocity.x > 0)
            //     {
            //         spriteRenderer.flipX = false;
            //     }
            //     else
            //     {
            //         spriteRenderer.flipX = true;
            //     }
            // }
        }
        
        return Vector2.zero;
    }

    // implement in each child class - calculates if it is seeking, fleeing, etc
    protected abstract Vector2 CalculateBehavior();


    

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
        Vector2 directionToCenter = (transform.position - Player.main.transform.position).normalized;

        // Dynamically adjust the stop distance based on player speed or other factors
        float playerSpeed = Player.main.GetComponent<Rigidbody2D>().velocity.magnitude;
        float dynamicDistance = endDistance + (playerSpeed * 0.1f); // Scale distance by player speed

        Vector2 stopPoint = (Vector2)Player.main.transform.position + (directionToCenter * dynamicDistance);

        return stopPoint;
    }



    // Seek
    public Vector2 Seek(Vector2 targetPos)
    {
        // Seek is a - b vector subraction for desired velocity
        Vector2 desiredVelocity = targetPos - (Vector2)transform.position;  // Calculate desired velocity

        desiredVelocity = desiredVelocity.normalized * maxSpeed;  // Set desired = max speed

        Vector2 seekingForce = desiredVelocity - rb.velocity;  // Calculate seek steering force

        return seekingForce;  // Return seek steering force
    }

    // Override Seek
    public Vector2 Seek(Critter target)
    {
        return Seek(target.transform.position);
    }

    // Flee
    public Vector2 Flee(Vector2 targetPos)
    {
        // Flee is b - a vector subtraction for desired velocity
        Vector2 desiredVelocity = (Vector2)transform.position - targetPos;  // Calculate desired velocity

        desiredVelocity = desiredVelocity.normalized * maxSpeed;  // Set desired = max speed

        Vector2 fleeingForce = desiredVelocity - rb.velocity;  // Calculate flee steering force

        return fleeingForce;  // Return flee steering force
    }

    // Override Flee
    public Vector2 Flee(Critter target)
    {
        return Flee(target.transform.position);
    }

    // Wander
    public Vector2 Wander(float time, float radius)
    {
        Vector2 futurePosition = CalcFuturePosition(time);

        randWanderAngle += UnityEngine.Random.Range(-10f, 10f);   // want in radians for later

        Vector2 wanderTarget = futurePosition;  // set wanderTarget to future position to make it easier later

        wanderTarget.x += Mathf.Cos(randWanderAngle * Mathf.Deg2Rad) * radius;
        wanderTarget.y += Mathf.Sin(randWanderAngle * Mathf.Deg2Rad) * radius;

        return Seek(wanderTarget);
    }






    // Separation
    public Vector2 Separation(List<Critter> agents)
    {
        Vector2 separationForce = Vector2.zero;

        foreach (Critter agent in agents)
        {
            float distance = Vector2.Distance(transform.position, agent.transform.position);  //find distance between agent and neighbors

            if (distance < 0.5f && agent != this)
            {
                separationForce = Flee(agent);
            }
        }
        return separationForce;
    }

    // Cohesion
    public Vector2 Cohesion(List<Critter> critters)
    {
        Vector2 cohesionForce = Vector2.zero;

        Vector2 posVectorSum = Vector2.zero;

        int count = 0;

        foreach (Critter critter in critters)
        {
            float distance = Vector2.Distance(transform.position, critter.transform.position);  //find distance between agent and neighbors

            if (distance < maxDisBetweenCritters && critter != this)
            {
                posVectorSum += (Vector2)critter.transform.position;
                count++;
            }

            Vector2 centroid = posVectorSum / count;

            cohesionForce = Seek(centroid);

        }

        return cohesionForce;
    }

    // Alignment
    public Vector2 Alignment(List<Critter> critters)
    {
        Vector2 alignmentForce = Vector2.zero;

        Vector2 velocitySum = Vector2.zero;

        foreach (Critter critter in critters)
        {
            float distance = Vector2.Distance(transform.position, critter.transform.position);  //find distance between agent and neighbors

            if (distance < maxDisBetweenCritters && critter != this)
            {
                velocitySum += critter.rb.velocity;
            }
        }

        Vector2 desiredVelocity = velocitySum.normalized * maxSpeed;

        alignmentForce = desiredVelocity - rb.velocity;

        return alignmentForce;
    }




    /*
         ------ Player Tools Interacting with Critters ------
    */


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
        if (Player.inventory.collectedCritters.Contains(critterData)) {
            // Don't want to catch the same critter twice
            return;
        }
        Player.inventory.AddCritter(critterData);
        // CritterManager.DeleteCritter(this);
        Destroy(gameObject);
        CatchCritterDialogBehavior.Show(critterData);
    }

    // When the critter is hit with a knife, call Knockback()
    public virtual void OnKnifeHit()
    {
        if (!knockedOut) {
            StartCoroutine(KnockBack(true, 250f, Player.main.transform.position));
        }
    }

    // critter gets knocked back and loses stamina
    IEnumerator KnockBack(bool damage, float knockBackAmount, Vector3 from)
    {
        freeBody = true;


        if (damage) {
            // make red color and turn sprite red
            spriteRenderer.color = Color.red;
            health -= 10;

            if (health <= 0) {
                knockedOut = true;
                rb.drag = 5f;

                GetComponent<ParticleSystem>().Play();
                spriteRenderer.color = Color.clear;
                yield return new WaitForSeconds(1.5f);

                CritterManager.DeleteCritter(this);
                Destroy(gameObject);
            }

        }

        ApplyForce((transform.position - from).normalized * knockBackAmount);
        yield return new WaitForSeconds(0.5f);

        spriteRenderer.color = Color.white;

        freeBody = false;
    }



    public void BounceAway(Vector2 from) {
        if (!knockedOut) {
            StartCoroutine(KnockBack(false, 500f, from));
        }
    }


    // calls DamagePlayer if an attack critter is within endDistance
    public virtual void AttackCritterHit()
    {
        // if distance bettween critter and Player is 
        if (Vector2.Distance(transform.position, Player.main.transform.position) < endDistance)
        {
            StartCoroutine(DamagePlayer());
        }
    }

    // Subtracts health from player
    IEnumerator DamagePlayer()
    {
        if (Player.main.GetComponent<Player>().health <= 0)
        {
            Debug.Log("Player dead");
        }

        // make red color and turn sprite red
        Player.main.GetComponent<Player>().SpriteRenderer.color = Color.red;

        Player.main.GetComponent<Player>().health -= 10;

        yield return new WaitForSeconds(1.0f);

        // return sprite color back to normal
        Player.main.GetComponent<Player>().SpriteRenderer.color = Color.white;
    }


}