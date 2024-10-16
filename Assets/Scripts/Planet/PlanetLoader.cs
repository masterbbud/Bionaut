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
    public int[] loopAroundIgnoreLayers = new int[] { };
    public GameObject interactionParticlePrefab;
    public static float planetHeight = 20;
    public static float planetWidth = 30;
    public float thisPlanetHeight;
    public float thisPlanetWidth;

    private float gridSize = 1f;  // Control the size of grid cells

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
        // Apply LoopAroundPlanet script to all looping objects on the planet
        foreach (GameObject childObject in gameObject.scene.GetRootGameObjects())
        {
            if (!loopAroundIgnoreLayers.Contains(childObject.layer))
            {
                if (!childObject.GetComponent<LoopAroundPlanet>())
                {
                    childObject.AddComponent<LoopAroundPlanet>();
                }
            }
        }
    }

    // Draw a grid outline in the Scene view using Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;  // Set the color of the grid

        // Draw the outer boundary of the planet
        Vector3 bottomLeft = transform.position - new Vector3(planetWidth / 2, planetHeight / 2, 0);
        Vector3 bottomRight = transform.position + new Vector3(planetWidth / 2, -planetHeight / 2, 0);
        Vector3 topLeft = transform.position + new Vector3(-planetWidth / 2, planetHeight / 2, 0);
        Vector3 topRight = transform.position + new Vector3(planetWidth / 2, planetHeight / 2, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // Draw internal grid lines
        for (float x = -planetWidth / 2; x <= planetWidth / 2; x += gridSize)
        {
            Vector3 start = transform.position + new Vector3(x, -planetHeight / 2, 0);
            Vector3 end = transform.position + new Vector3(x, planetHeight / 2, 0);
            Gizmos.DrawLine(start, end);
        }

        for (float y = -planetHeight / 2; y <= planetHeight / 2; y += gridSize)
        {
            Vector3 start = transform.position + new Vector3(-planetWidth / 2, y, 0);
            Vector3 end = transform.position + new Vector3(planetWidth / 2, y, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}
