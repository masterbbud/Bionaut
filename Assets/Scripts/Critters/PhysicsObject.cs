using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    Vector3 position, direction, velocity, acceleration;

    public Vector3 Velocity
    {
        get { return velocity; }
    }

    public Vector3 Direction
    {
        get { return direction; }
    }

    [SerializeField]
    float mass = 1f, maxSpeed;    // mass is 1 because default float value is 0 which would end up having division by 0

    public float MaxSpeed
    {
        get { return maxSpeed; }
    }

    public float radius = 1f;

    public bool doesRotate = true;


    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // calculate the velocity for this frame - New
        velocity += acceleration * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        position += velocity * Time.deltaTime;

        // grab current direction from velocity - New
        direction = velocity.normalized;

        transform.position = position;

        if (doesRotate == true)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.back, direction);

        }

        // zero out acceleration - New
        acceleration = Vector3.zero;
    }



    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;   // += REALLY IMPORTANT!
    }

}
