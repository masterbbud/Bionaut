using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CritterManager : Singleton<CritterManager>
{

    [SerializeField]
    Critter tempCritter;

    [SerializeField]
    int tempCritterCount;

    private Collider2D spawnArea;
    

    // critter list
    public static List<Critter> critters = new List<Critter>();

    Vector2 screenSize = Vector2.zero;

    public Vector2 ScreenSize { get { return screenSize; } }


    // prevent non singleton constructor use
    protected CritterManager() { }

    private void Start()
    {
        screenSize.y = Camera.main.orthographicSize;
        screenSize.x = screenSize.y * Camera.main.aspect;

        critters = new List<Critter>();

        spawnArea = GetComponent<Collider2D>();

        Spawn();
    }


    void Spawn()
    {
        // critter spawning
        for (int i = 0; i < tempCritterCount; i++)
        {
            Critter newTempCritter = Instantiate(tempCritter, PickRandomPoint(), Quaternion.identity);
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
        do {
            randPoint.x = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
            randPoint.y = Random.Range(spawnBounds.min.y, spawnBounds.max.y);
        } while (spawnArea.ClosestPoint(randPoint) != randPoint);

        return randPoint;

    }
}

