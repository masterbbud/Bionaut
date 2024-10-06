using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
* Contains all static references for the planet.
* This includes the planet size and prefabs such as particles
* that need to get referenced by other classes.
*/
public class PlanetLoader : MonoBehaviour
{
    public int[] loopAroundIgnoreLayers = new int[]{};
    public GameObject interactionParticlePrefab;
    public static float planetHeight = 20;
    public static float planetWidth = 30;
    public float thisPlanetHeight;
    public float thisPlanetWidth;
    void Awake()
    {
        // Set up planet height and width for reference
        planetHeight = thisPlanetHeight;
        planetWidth = thisPlanetWidth;

        // Inject prefabs into other classes statically
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
