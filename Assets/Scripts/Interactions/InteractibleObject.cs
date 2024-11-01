using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * This should be inherited by any object which can be interacted with.
 * Applies particles to the object which glow when the player is nearby.
 * Override Interact() to provide custom interaction behavior.
 */
public abstract class InteractibleObject : MonoBehaviour
{
    public static List<InteractibleObject> interactions = new List<InteractibleObject>();
    public static GameObject particlePrefab;
    private ParticleSystem particles;

    protected bool hasSparkles = true;

    // Start is called before the first frame update
    public virtual void Start()
    {
        particles = Instantiate(particlePrefab, transform).GetComponent<ParticleSystem>();
        var particleShape = particles.shape;
        particleShape.spriteRenderer = GetComponent<SpriteRenderer>();
        var emission = particles.emission;
        emission.rateOverTime = 4;
        var main = particles.main;
        main.startSize = 0.2f;
        particles.GetComponent<Renderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // If the player's interaction has walked nearby, start glowing brighter
        if (other.gameObject == InteractionCollider.main) {
            OnPlayerNear();
            var emission = particles.emission;
            emission.rateOverTime = 8;
            var main = particles.main;
            main.startSize = 0.3f;
            interactions.Add(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // If the player's interaction has walked away, stop glowing brighter
        if (other.gameObject == InteractionCollider.main) {
            OnPlayerLeave();
            var emission = particles.emission;
            emission.rateOverTime = 4;
            var main = particles.main;
            main.startSize = 0.2f;
            interactions.Remove(this);
        }
    }

    protected virtual void OnPlayerNear(){}
    protected virtual void OnPlayerLeave(){}

    void OnMouseOver()
    {
        Debug.Log("over");
        // Interact only if the player is close enough
        if (Input.GetMouseButtonDown(1) && interactions.Contains(this)) {
            Interact();
        }
    }

    public abstract void Interact();
}
