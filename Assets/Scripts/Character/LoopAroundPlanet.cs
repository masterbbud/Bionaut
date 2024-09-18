using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This class should be applied to any gameObject which "exists" on the planet.
 * It causes its sprite to stay locked to the "correct" plane, and also duplicates its sprite so that
 * it can be seen on all sides of the planet.
 */
public class LoopAroundPlanet : MonoBehaviour
{
    private float planetWidth = PlanetManager.planetWidth;
    private float planetHeight = PlanetManager.planetHeight;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x != 0 || y != 0) {
                    // This is a pretty bad method of copying the Component over. It would be nice to copy the component,
                    // but that isn't built-in
                    GameObject newGameObject = new GameObject("subsprite");
                    newGameObject.transform.parent = transform;
                    newGameObject.transform.position = transform.position + new Vector3(x*planetWidth, y*planetHeight);
                    newGameObject.transform.localScale = new Vector3(1,1,1);
                    // newGameObject.transform.rotation = transform.rotation;
                    newGameObject.AddComponent<SpriteRenderer>();
                    SpriteRenderer sr = newGameObject.GetComponent<SpriteRenderer>();
                    SpriteRenderer mySR = GetComponent<SpriteRenderer>();
                    sr.sprite = mySR.sprite;
                    sr.sortingOrder = mySR.sortingOrder;
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
