using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // DONT use a maxDistance > planetWidth/2 or > planetHeight/2, since this will cause the bullet to never disappear
    [SerializeField]
    private float maxDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the projectile if it's too far from the player, that way it doesn't loop around the planet
        if (Vector2.Distance(Player.main.transform.position, transform.position) > maxDistance) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        // If the object hit was hittable by a rifle, apply the rifle's effects to the object
        IRifleHittable hittable = coll.GetComponent<IRifleHittable>();
        if (hittable != null) {
            hittable.OnRifleHit();
        }

        // In any case, destroy the projectile
        // NOTE: It's important that the "projectile" layer is managed well so that projectiles don't destroy themselves
        // when hitting invisible triggers
        Destroy(gameObject);
    }
}
