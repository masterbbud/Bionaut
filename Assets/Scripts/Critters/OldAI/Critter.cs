using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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

    [SerializeField]
    float maxDisBetweenCritters = 1.0f;  // used for separation, cohesion, and alignment

    float randWanderAngle;  // used in wander

    protected Vector2 min, max;  // used with with the sprite renderer in child classes

    private bool asleep = false;   // was the critter hit

    public CritterData critterData;

    // While true, the critter doesn't move on its own and can go beyond its
    // max speed.
    private bool freeBody = false;    // ??
    private bool knockedOut = false;   // is the critter knocked out
    private int stamina;   // like health?
    private bool activeflee = false;

    [SerializeField]
    float mass = 1f;  // mass of critter; used in ApplyForces()

    [SerializeField]
    protected float maxSpeed;  // maxSpeed of critter

    // Instance of the audio manager
    public AudioManager audioManager;

    public Animator animator;
    public Vector3 facingDirection = Vector3.zero;


    /*
          ----------- Methods -----------
    */


    // Start Method
    void Start()
    {
        randWanderAngle = UnityEngine.Random.Range(0f, 360f);
        StartSubclass();
    }

    // Outlined within the childclasses
    protected virtual void StartSubclass() {}

    // Update Method
    void Update()
    {
        if (!freeBody && !knockedOut) {
            totalForces += CalculateSteeringForces();

            totalForces = Vector2.ClampMagnitude(totalForces, maxForce);

            ApplyForce(totalForces);
        }
        
        totalForces = Vector2.zero;

        UpdateAnimation();
    }


    // Update Animation Method
    void UpdateAnimation()
    {
        bool isMoving = rb.velocity.sqrMagnitude > 0.0f;
        animator.SetBool("IsFleeing", isMoving);
        if(isMoving)
        {
            animator.SetFloat("Horizontal", rb.velocity.x);
            animator.SetFloat("Vertical", rb.velocity.y);
        }

        // We want to use the facing direction based on the player moving direction

        // if (rb.velocity != Vector2.zero)
        // {
        //     double angle = Math.Atan2(rb.velocity.y, rb.velocity.x);
        //     angle /= Math.PI / 2;
        //     if (angle == 1.5 || angle == -0.5)
        //     {
        //         angle += 0.5;
        //         // The animator treats direction slightly differently, so we have to do this to prioritize
        //         // the sideways angles
        //     }
        //     angle = Math.Floor(angle);
        //     angle *= Math.PI / 2;
        //     facingDirection = new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);

        //     // TODO this has imperfect behavior when traveling against a wall
        // }
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
        activeflee = true;

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
            StartCoroutine(KnockBack());
        }
    }

    // critter gets knocked back and loses stamina
    IEnumerator KnockBack()
    {
        freeBody = true;
        float knockBackAmount = 250f;
        stamina -= 1;
        Debug.Log(stamina);
        if (stamina <= 0) {
            // We have to do this bc color is a readonly field
            Color c = spriteRenderer.color;
            c = new Color(0.6f, 0.6f, 0.6f);
            spriteRenderer.color = c;
        }
        ApplyForce((transform.position - Player.main.transform.position).normalized * knockBackAmount);
        yield return new WaitForSeconds(0.5f);
        
        if (stamina <= 0) {
            knockedOut = true;
            rb.drag = 5f;
        }
        freeBody = false;
    }


}