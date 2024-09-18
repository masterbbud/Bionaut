using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This class should be applied to any gameObject which "exists" on the planet.
 * It causes its sprite to stay locked to the "correct" plane, and also duplicates its sprite so that
 * it can be seen on all sides of the planet.
 */
public class TilemapLoop : MonoBehaviour
{
    private float planetWidth = PlanetManager.planetWidth;
    private float planetHeight = PlanetManager.planetHeight;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.childCount > 1) {
            return;
        }
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x != 0 || y != 0) {
                    GameObject newGameObject = Instantiate(gameObject, transform.parent);
                    newGameObject.transform.position = transform.position + new Vector3(x*planetWidth, y*planetHeight);
                    newGameObject.GetComponent<TilemapLoop>().enabled = false;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (transform.position.y > planetHeight / 2) 
        {
            TransformSelf(0, -planetHeight);
        } else if (transform.position.y < -planetHeight / 2) 
        {
            TransformSelf(0, planetHeight);
        }

        if (transform.position.x > planetWidth / 2)
        {
            TransformSelf(-planetWidth, 0);
        } else if (transform.position.x < -planetWidth / 2)
        {
            TransformSelf(planetWidth, 0);
        }
    }
    void TransformSelf(float x, float y)
    {
        transform.position += new Vector3(x, y, 0);
    }
}
