using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CritterManager : Singleton<CritterManager>
{

    [SerializeField]
    Critter tempCritter;

    [SerializeField]
    int tempCritterCount;
    

    // critter list
    public List<Critter> critters = new List<Critter>();

    Vector2 screenSize = Vector2.zero;

    public Vector2 ScreenSize { get { return screenSize; } }


    // prevent non singleton constructor use
    protected CritterManager() { }

    private void Start()
    {
        screenSize.y = Camera.main.orthographicSize;
        screenSize.x = screenSize.y * Camera.main.aspect;

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
        Vector2 randPoint = Vector2.zero;

        randPoint.x = Random.Range(-ScreenSize.x, ScreenSize.x);
        randPoint.y = Random.Range(-ScreenSize.y, ScreenSize.y);

        return randPoint;

    }




}

