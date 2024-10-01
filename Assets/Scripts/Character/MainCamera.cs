using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    void Start()
    {
        
    }

    // Follow the player
    void Update()
    {
        transform.position = new Vector3(Player.main.transform.position.x, Player.main.transform.position.y, transform.position.z);
    }
}
