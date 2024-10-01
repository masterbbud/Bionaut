using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Critter : MonoBehaviour, IRifleHittable
{

    protected Vector2 totalForces = Vector2.zero;

    [SerializeField]
    float maxForce = 5f;

    [SerializeField]
    float maxDistance = 1.0f;

    float randAngle;

    [SerializeField]
    float safeDis;

    public Vector2 min, max;

    private bool asleep = false;

    // list of all positions of obstacles agent has found (each agent has there own list
    protected List<Vector2> foundObstaclePositions = new List<Vector2>();


    // Start is called before the first frame update
    void Start()
    {
        randAngle = Random.Range(0f, 360f);
    }

    // Update is called once per frame
    void Update()
    {
        // parent class does not care what the function does, that depends on code in child class
        totalForces += CalculateSteeringForces();

        totalForces = Vector2.ClampMagnitude(totalForces, maxForce);

        ApplyForce(totalForces);

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


    // Evade calculates fleeing from a future position vs. flee is just fleeing in the opposite direction
    public Vector2 Evade(Critter target)
    {
        return Flee(target.CalcFuturePosition(5f));
    }


    // picks a somewhat random point to put into Seek() function to look like wandering
    public Vector2 Wander(float time, float radius)
    {
        Vector2 futurePosition = CalcFuturePosition(time);

        randAngle += Random.Range(-10f, 10f);   // want in radians for later

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
    public Vector2 Cohesion(List<Critter> agents)
    {
        Vector2 cohesionForce = Vector2.zero;

        Vector2 posVectorSum = Vector2.zero;

        int count = 0;

        foreach (Critter fish in agents)
        {
            float distance = Vector2.Distance(transform.position, fish.transform.position);  //find distance between agent and neighbors

            if (distance < maxDistance && fish != this)
            {
                posVectorSum += (Vector2)fish.transform.position;
                count++;
            }

            Vector2 centroid = posVectorSum / count;

            cohesionForce = Seek(centroid);

        }

        return cohesionForce;
    }

    // Alignment
    public Vector2 Alignment(List<Critter> agents)
    {
        Vector2 alignmentForce = Vector2.zero;

        Vector2 velocitySum = Vector2.zero;

        foreach (Critter fish in agents)
        {
            float distance = Vector2.Distance(transform.position, fish.transform.position);  //find distance between agent and neighbors

            if (distance < maxDistance && fish != this)
            {
                velocitySum += fish.rb.velocity;
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

    [SerializeField]
    protected Rigidbody2D rb;
    
    [SerializeField]
    float mass = 1f, maxSpeed;    // mass is 1 because default float value is 0 which would end up having division by 0

    public void ApplyForce(Vector2 force)
    {
        rb.velocity += (force / mass) * Time.deltaTime;   // += REALLY IMPORTANT!
        float currentMaxSpeed = maxSpeed;
        if (asleep) {
            currentMaxSpeed *= 0.02f;
        }
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, currentMaxSpeed);
    }


    public void OnRifleHit()
    {
        StartCoroutine(FallAsleep());
    }

    IEnumerator FallAsleep()
    {
        
        asleep = true;
        yield return new WaitForSeconds(5);
        asleep = false;
    }
}




/*   Stuff not needed rn

// -----------------------------------------------

 // checks if agent is in bounds for stay in bounds function
    protected bool CheckIfInBounds(Vector3 position)
    {
        if (position.x > AgentManager.Instance.ScreenSize.x ||
            position.x < -AgentManager.Instance.ScreenSize.x ||
            position.y > AgentManager.Instance.ScreenSize.y ||
            position.y < -AgentManager.Instance.ScreenSize.y)
        {
            return true;
        }

        return false;
    }

// -----------------------------------------------
 
 // stay in bounds of screen function
    public Vector3 StayInBounds()
    {
        Vector3 steeringForce = Vector3.zero;

        if (CheckIfInBounds(transform.position))
        {
            steeringForce += Seek(Vector3.zero);
        }

        return steeringForce;
    }
 
 // -----------------------------------------------

    // avoid obstacles
    public Vector3 AvoidObstacles()
    {
        foundObstaclePositions.Clear();

        Vector3 steeringForce = Vector3.zero;

        Vector3 vectorFromPlayerToPufferfish = Vector3.zero;

        float forwardDot, rightDot;

        Vector3 right = Vector3.Cross(Vector3.back, physicsObject.Direction);  // instead of transform.right

        foreach (Pufferfish pufferfish in AgentManager.Instance.pufferfishList)
        {
            vectorFromPlayerToPufferfish = pufferfish.transform.position - transform.position;

            // calc foward and right dot products
            forwardDot = Vector3.Dot(physicsObject.Direction, vectorFromPlayerToPufferfish);
            rightDot = Vector3.Dot(right, vectorFromPlayerToPufferfish);

            // calc safe distance
            float safe = safeDis + pufferfish.radius;

            // if obstacle is in front and within safe distance
            if (forwardDot > 0f && forwardDot < safeDis)
            {
                // if obstacle is within side bounds
                if (Mathf.Abs(rightDot) < pufferfish.radius + physicsObject.radius)
                {
                    // Found something to avoid
                    foundObstaclePositions.Add(pufferfish.transform.position);

                    // calc desired velocity
                    Vector3 desiredVelocity = right * -Mathf.Sign(rightDot) * physicsObject.MaxSpeed
                        * 1 / vectorFromPlayerToPufferfish.magnitude;  // when sensing multiple obstacles, avoid obstacle closest to you

                    // calc steering force
                    steeringForce += desiredVelocity - rb.velocity;
                }
            }
        }
        return steeringForce;
    }

// -----------------------------------------------

// -----------------------------------------------

// -----------------------------------------------
 
 
 
 */