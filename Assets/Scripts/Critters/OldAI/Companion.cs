using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Companion : Critter
{
    //[SerializeField]
    //SpriteRenderer spriteRenderer;

    [SerializeField]
    float seekWeight;

    [SerializeField]
    float distance;  // max distance that critter will flee or seek 

    public static GameObject main;


    void Start()
    {
        min = spriteRenderer.bounds.min;
        max = spriteRenderer.bounds.max;
        DontDestroyOnLoad(gameObject);
        main = gameObject;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "PlanetMapScene" || scene.name == "StartScene") {
            main.SetActive(false);
        }
        // Player should be active on all other scenes
        else {
            main.SetActive(true);
            main.transform.position = Player.main.transform.position;
        }
    }

    // this method was created in parent class but each child class has to implement it separately
    protected override Vector2 CalculateSteeringForces()
    {
        min = spriteRenderer.bounds.min;
        max = spriteRenderer.bounds.max;

        Vector2 seekForce = Seek(Player.main.transform.position) * seekWeight;


        if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            return -1 * rb.velocity;
        }

        return seekForce;
    }



    void OnMouseOver()
    {
        // Pseudo-interactible object, this triggers on right-click and does something but doesn't have sparkles
        if (Input.GetMouseButtonDown(1) && InteractionCollider.main.GetComponent<Collider2D>().IsTouching(GetComponent<Collider2D>())) {
            Interact();
        }
    }

    public void Interact()
    {
        GetComponent<ParticleSystem>().Play();
    }


    // Drawing Gizmos function
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
    
        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }
    }

    public override void OnKnifeHit() {}
    public override void OnRifleHit() {}
    public override void OnNetHit() {}
}