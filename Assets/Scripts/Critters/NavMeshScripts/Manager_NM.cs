using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Scripts for spawning critters of a specific type in a specific area.
 */
public class Manager_NM : MonoBehaviour
{

    [SerializeField]
    GameObject tempCritter;

    [SerializeField]
    int tempCritterCount;

    private Collider2D spawnArea;


    // critter list
    public static List<GameObject> critters = new List<GameObject>();

    // prevent non singleton constructor use
    protected Manager_NM() { }

    private void Start()
    {
        critters = new List<GameObject>();

        spawnArea = GetComponent<Collider2D>();

        Spawn();
    }


    void Spawn()
    {
        // critter spawning
        for (int i = 0; i < tempCritterCount; i++)
        {
            GameObject newTempCritter = Instantiate(tempCritter, PickRandomPoint(), Quaternion.identity);
            critters.Add(newTempCritter);
            newTempCritter.AddComponent<LoopAroundPlanet>();
        }

    }


    Vector2 PickRandomPoint()
    {
        Bounds spawnBounds = spawnArea.bounds;
        Vector2 randPoint = Vector2.zero;

        // Continually pick a random point in the x, y bounds of the spawn collider
        // until the point falls inside the collider
        do
        {
            randPoint.x = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
            randPoint.y = Random.Range(spawnBounds.min.y, spawnBounds.max.y);
        } while (spawnArea.ClosestPoint(randPoint) != randPoint);

        return randPoint;

    }

    // Deletes a critter from the list - this should ALWAYS be done when a critter is destroyed
    public static void DeleteCritter(GameObject critter)
    {
        critters.Remove(critter);
    }
}
