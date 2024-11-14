using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompanionNew : CritterAI
{
    public static GameObject main;


    void Start()
    {
        DontDestroyOnLoad(gameObject);
        main = gameObject;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }



    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlanetMapScene" || scene.name == "StartScene")
        {
            main.SetActive(false);
        }
        // Player should be active on all other scenes
        else
        {
            main.SetActive(true);
            main.transform.position = Player.main.transform.position;
        }
    }


    
    void OnMouseOver()
    {
        // Pseudo-interactible object, this triggers on right-click and does something but doesn't have sparkles
        if (Input.GetMouseButtonDown(1) && InteractionCollider.main.GetComponent<Collider2D>().IsTouching(GetComponent<Collider2D>()))
        {
            Interact();
        }
    }

    public void Interact()
    {
        GetComponent<ParticleSystem>().Play();
    }


    

    //public override void OnKnifeHit() { }
    //public override void OnRifleHit() { }
    //public override void OnNetHit() { }


























Vector2 stopPoint;   // point for critter to seek/stop around player

    // sets seek point to Player
    protected override Vector2 CalculateBehavior()
    {
        stopPoint = StopAttackPoint();
        return stopPoint;

    }


    // Gizmos Method
    private void OnDrawGizmos()
    {
        if (Vector2.Distance(transform.position, Player.main.transform.position) < endDistance)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }

        //Gizmos.color = Color.red;
        //Vector2 directionToCenter = (transform.position - Player.main.transform.position).normalized;
        //Vector2 closestPoint = (Vector2)Player.main.transform.position + (directionToCenter * endDistance);
        //Gizmos.DrawLine(transform.position, closestPoint);
    }

}
