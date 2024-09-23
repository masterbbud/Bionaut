using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractibleObject : MonoBehaviour
{
    public static List<InteractibleObject> interactions = new List<InteractibleObject>();
    public static GameObject particlePrefab;
    private ParticleSystem particles;
    // Start is called before the first frame update
    void Start()
    {
        particles = Instantiate(particlePrefab, transform).GetComponent<ParticleSystem>();
        var particleShape = particles.shape;
        particleShape.spriteRenderer = GetComponent<SpriteRenderer>();
        var emission = particles.emission;
        emission.rateOverTime = 3;
        var main = particles.main;
        main.startSize = 0.1f;
        particles.GetComponent<Renderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == InteractionCollider.main) {
            var emission = particles.emission;
            emission.rateOverTime = 8;
            var main = particles.main;
            main.startSize = 0.2f;
            interactions.Add(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == InteractionCollider.main) {
            var emission = particles.emission;
            emission.rateOverTime = 3;
            var main = particles.main;
            main.startSize = 0.1f;
            interactions.Remove(this);
        }
    }

    public abstract void Interact();
}
