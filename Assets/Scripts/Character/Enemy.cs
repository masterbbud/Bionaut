using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAIState {
    Wander = 0,
    Chase = 1,
}

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private Character player;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float visionRadius;

    [SerializeField]
    private float chaseRadius;

    private EnemyAIState aiState = EnemyAIState.Wander;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerPos = GetNearestOfTransform(transform.position, player.gameObject.transform.position);
        Vector2 currentPos = gameObject.transform.position;

        if (Vector2.Distance(playerPos, currentPos) < visionRadius) {
            aiState = EnemyAIState.Chase;
        }

        if (Vector2.Distance(playerPos, currentPos) > chaseRadius) {
            aiState = EnemyAIState.Wander;
        }

        Vector2 headingTo;

        if (aiState == EnemyAIState.Chase) {
            headingTo = playerPos;

            // apply AStar to chase player
        }
        else if (aiState == EnemyAIState.Wander) {
            headingTo = currentPos;
        }
        else {
            headingTo = currentPos;
        }

        // raycast to player to check los
        // if in active state, follow player through walls

        Vector2 heading = (headingTo - currentPos).normalized;

        GetComponent<Rigidbody2D>().velocity = (Vector3)heading * speed;
    }

    /*
     * Probably should be a static function
     * Gets the nearest `otherTransform` to the given `initialTransform`.
     * For example, a monster at `initialTransform` that wants to go towards `otherTransform`
     * should head towards the result of this function.
     */
    Vector2 GetNearestOfTransform(Vector2 initialTransform, Vector2 otherTransform)
    {
        float planetWidth = PlanetManager.planetWidth;
        float planetHeight = PlanetManager.planetHeight;

        // Get player position in the central screen
        Vector2 playerPos = new Vector2((otherTransform.x + planetWidth) % planetWidth, (otherTransform.y + planetHeight) % planetHeight);

        // Loop over positions in adjancent screens to find the translation that is the closest to your player
        Vector2 bestPos = playerPos;
        float bestDist = planetHeight + planetWidth; // greater than any other possible distance
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                Vector2 possibleBestPos = playerPos + new Vector2(x * planetWidth, y * planetHeight);
                if (Vector2.Distance(initialTransform, possibleBestPos) < bestDist) {
                    bestPos = possibleBestPos;
                    bestDist = Vector2.Distance(initialTransform, possibleBestPos);
                }
            }
        }

        return bestPos;
    }
}
