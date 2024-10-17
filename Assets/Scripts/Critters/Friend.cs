using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Scripts specific to your companion critter
 */
public class Friend : Critter
{
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

    public override void OnNetHit() {}
    public override void OnKnifeHit() {}
    public override void OnRifleHit() {}
}
