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

    protected Vector2 totalForces = Vector2.zero;

    [SerializeField]
    float maxForce = 5f;

    [SerializeField]
    float maxDistance = 1.0f;

    float randAngle;

    protected Vector2 min, max;

    private bool asleep = false;

    public CritterData critterData;
    // While true, the critter doesn't move on its own and can go beyond its
    // max speed.
    private bool freeBody = false;
    private bool knockedOut = false;
    private int stamina;
    
    [SerializeField]
    protected Rigidbody2D rb;

    [SerializeField]
    float mass = 1f;

    [SerializeField]
    protected float maxSpeed;    // mass is 1 because default float value is 0 which would end up having division by 0

    [SerializeField]
    protected int maxStamina;

    // Instance of the audio manager
    public AudioManager audioManager;


    // Start is called before the first frame update
    void Start()
    {
        randAngle = UnityEngine.Random.Range(0f, 360f);
        stamina = maxStamina;
        StartSubclass();
    }

    protected virtual void StartSubclass() {}

    // Update is called once per frame
    void Update()
    {
        if (!freeBody && !knockedOut) {
            totalForces += CalculateSteeringForces();

            totalForces = Vector2.ClampMagnitude(totalForces, maxForce);

            ApplyForce(totalForces);
        }
        
        totalForces = Vector2.zero;
    }

    // abstract method = you implement the function in each child class
    protected abstract Vector2 CalculateSteeringForces();



    public Vector2 Seek(Vector2 targetPos)
    {
        // Seek is a - b vector subraction for desired velocity
        Vector2 desiredVelocity = targetPos - (Vector2)transform.position;  // Calculate desired velocity

        desiredVelocity = desiredVelocity.normalized * maxSpeed;  // Set desired = max speed

        Vector2 seekingForce = desiredVelocity - rb.velocity;  // Calculate seek steering force

        return seekingForce;  // Return seek steering force
    }

    // overrides function above to make it easier in seeker script
    public Vector2 Seek(Critter target)
    {
        return Seek(target.transform.position);
    }


    public Vector2 Flee(Vector2 targetPos)
    {
        // Flee is b - a vector subtraction for desired velocity
        Vector2 desiredVelocity = (Vector2)transform.position - targetPos;  // Calculate desired velocity

        desiredVelocity = desiredVelocity.normalized * maxSpeed;  // Set desired = max speed

        Vector2 fleeingForce = desiredVelocity - rb.velocity;  // Calculate flee steering force

        return fleeingForce;  // Return flee steering force
    }

    // overrides function above to make it easier in fleer script
    public Vector2 Flee(Critter target)
    {
        return Flee(target.transform.position);
    }


    // picks a somewhat random point to put into Seek() function to look like wandering
    public Vector2 Wander(float time, float radius)
    {
        Vector2 futurePosition = CalcFuturePosition(time);

        randAngle += UnityEngine.Random.Range(-10f, 10f);   // want in radians for later

        Vector2 wanderTarget = futurePosition;  // set wanderTarget to future position to make it easier later

        wanderTarget.x += Mathf.Cos(randAngle * Mathf.Deg2Rad) * radius;
        wanderTarget.y += Mathf.Sin(randAngle * Mathf.Deg2Rad) * radius;

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

            if (distance < maxDistance && critter != this)
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

            if (distance < maxDistance && critter != this)
            {
                velocitySum += critter.rb.velocity;
            }
        }

        Vector2 desiredVelocity = velocitySum.normalized * maxSpeed;

        alignmentForce = desiredVelocity - rb.velocity;

        return alignmentForce;
    }


    
    // calculates the future position of agent
    public Vector2 CalcFuturePosition(float futureTime)
    {
        return (Vector2)transform.position + (rb.velocity * futureTime);  // returns a world point because of the transform.position
    }

    public void ApplyForce(Vector2 force)
    {
        rb.velocity += (force / mass) * Time.deltaTime;   // += REALLY IMPORTANT!
        float currentMaxSpeed = maxSpeed;
        if (asleep) {
            currentMaxSpeed *= 0.02f;
        }
        if (!freeBody) {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, currentMaxSpeed);
        }
    }


    // When the critter is hit with a rifle, it should fall asleep
    public virtual void OnRifleHit()
    {
        StartCoroutine(FallAsleep());
    }

    IEnumerator FallAsleep()
    {
        asleep = true;
        yield return new WaitForSeconds(5);
        asleep = false;
    }

    // When the critter is hit with a net, the player should catch it
    public virtual void OnNetHit()
    {
        // Player catches this critter!
        if (Player.inventory.collectedCritters.Contains(critterData)) {
            // Don't want to catch the same critter twice
            return;
        }
        Player.inventory.AddCritter(critterData);
        CritterManager.DeleteCritter(this);
        Destroy(gameObject);
        CatchCritterDialogBehavior.Show(critterData);
    }

    // When the critter is hit with a knife, it should get pushed back and
    // lose stamina
    public virtual void OnKnifeHit()
    {
        if (!knockedOut) {
            StartCoroutine(KnockBack());
        }
    }

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