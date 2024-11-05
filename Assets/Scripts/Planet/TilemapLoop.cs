using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/*
 * This class should be applied to SPECIFICALLY the tilemap on a planet.
 */
public class TilemapLoop : MonoBehaviour
{
    private float planetWidth = PlanetLoader.planetWidth;
    private float planetHeight = PlanetLoader.planetHeight;
    // Start is called before the first frame update
    void Start()
    {
        planetHeight = PlanetLoader.planetHeight;
        planetWidth = PlanetLoader.planetWidth;
        
        if (transform.parent.Find(name + "copy")) {
            return;
        }

        // We save the copy reference to prevent the objects from duplicating the new copies
        GameObject referenceToCopy = null;

        // We have to copy the tilemap looped exactly the planet distance away in all 8 cardinal directions because as the player
        // moves close to the edge of the screen, they will see the next loop
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x != 0 || y != 0) {
                    GameObject newGameObject;
                    if (referenceToCopy) {
                        newGameObject = Instantiate(referenceToCopy, transform);
                    }
                    else {
                        newGameObject = Instantiate(gameObject, transform);
                        referenceToCopy = newGameObject;
                    }
                    newGameObject.name = name + "copy";
                    newGameObject.transform.position = transform.position + new Vector3(x*planetWidth, y*planetHeight);
                    newGameObject.GetComponent<TilemapLoop>().enabled = false;
                }
            }
        }

        StartCoroutine(FlashColliders());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        // If the "central" object is more than half the planet size away from the player,
        // we move it so that it is closer than half the planet size away
        Vector2 transformToPlayer = transform.position - Player.main.transform.position;
        if (transformToPlayer.y > planetHeight / 2) 
        {
            TransformSelf(0, -planetHeight);
        } else if (transformToPlayer.y < -planetHeight / 2) 
        {
            TransformSelf(0, planetHeight);
        }

        if (transformToPlayer.x > planetWidth / 2)
        {
            TransformSelf(-planetWidth, 0);
        } else if (transformToPlayer.x < -planetWidth / 2)
        {
            TransformSelf(planetWidth, 0);
        }
    }

    IEnumerator FlashColliders() {
        
        yield return new WaitForSeconds(0.2f);

        TilemapCollider2D coll = GetComponent<TilemapCollider2D>();
        if (coll) {
            coll.enabled = true;
        }
    }

    void TransformSelf(float x, float y)
    {
        transform.position += new Vector3(x, y, 0);
    }
}
