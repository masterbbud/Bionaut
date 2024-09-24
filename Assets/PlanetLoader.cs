using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetLoader : MonoBehaviour
{
    public int[] loopAroundIgnoreLayers = new int[]{};
    public GameObject interactionParticlePrefab;
    void Awake()
    {
        InteractibleObject.particlePrefab = interactionParticlePrefab;
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadPlanet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadPlanet() 
    {
        // apply looparoundplanet script to all looping objects on the planet
        foreach (GameObject childObject in gameObject.scene.GetRootGameObjects()) {
            if (!loopAroundIgnoreLayers.Contains(childObject.layer)) {
                if (!childObject.GetComponent<LoopAroundPlanet>()) {
                    childObject.AddComponent<LoopAroundPlanet>();
                }
            }
            
        }
    }
}
