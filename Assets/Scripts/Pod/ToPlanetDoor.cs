using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToPlanetDoor : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Player.main)
        {
            // Load the scene for the selected planet
            Debug.Log($"Entering Planet Scene: {UIPlanet.selectedPlanetScene}");
            SceneManager.LoadScene(UIPlanet.selectedPlanetScene);
        }
    }
}
