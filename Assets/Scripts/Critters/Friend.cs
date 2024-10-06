using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : Critter
{
    void OnMouseOver()
    {
        Debug.Log(InteractionCollider.main.GetComponent<Collider2D>());
        Debug.Log(GetComponent<Collider2D>());
        if (Input.GetMouseButtonDown(1) && InteractionCollider.main.GetComponent<Collider2D>().IsTouching(GetComponent<Collider2D>())) {
            Interact();
        }
    }

    public void Interact()
    {
        GetComponent<ParticleSystem>().Play();
    }
}
