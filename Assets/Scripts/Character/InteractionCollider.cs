using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The script for the collider around the Player
 * which determines if the Player can interact with objects
 */
public class InteractionCollider : MonoBehaviour
{
    public static GameObject main;
    // Start is called before the first frame update
    void Start()
    {
        main = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
