using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CritterManager1 : MonoBehaviour
{
    [SerializeField]
    List<Critter> critterPrefabs; // List of critter prefabs to spawn

    [SerializeField]
    List<int> critterCounts; // Number of each critter type to spawn

    [SerializeField]
    AudioManager audioManager;

    private Collider2D spawnArea;

    // Static list of critters
    public static List<Critter> critters = new List<Critter>();

    protected CritterManager1() { }

    private void Start()
    {
        critters = new List<Critter>();
        spawnArea = GetComponent<Collider2D>();

        Spawn();
    }

    void Spawn()
    {
        for (int i = 0; i < critterPrefabs.Count; i++)
        {
            for (int j = 0; j < critterCounts[i]; j++)
            {
                SpawnCritter(critterPrefabs[i]);
            }
        }
    }

    public void SpawnCritter(Critter critterPrefab)
    {
        Critter newCritter = Instantiate(critterPrefab, PickRandomPoint(), Quaternion.identity);
        critters.Add(newCritter);
        newCritter.AddComponent<LoopAroundPlanet>();

        // Link to the audio manager
        newCritter.audioManager = audioManager;
    }

    Vector2 PickRandomPoint()
    {
        Bounds spawnBounds = spawnArea.bounds;
        Vector2 randPoint;

        do
        {
            randPoint.x = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
            randPoint.y = Random.Range(spawnBounds.min.y, spawnBounds.max.y);
        } while (spawnArea.ClosestPoint(randPoint) != randPoint);

        return randPoint;
    }

    public static void DeleteCritter(Critter critter)
    {
        critters.Remove(critter);
    }

    private void OnDrawGizmos()
    {
        if (spawnArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);
        }
    }
}
