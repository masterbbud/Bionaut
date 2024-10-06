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
    [SerializeField]
    private bool shouldCreateCopies = false;
    // Start is called before the first frame update
    void Start()
    {
        if (shouldCreateCopies)
        {
            CreateCopies();
        }
    }
    
    // Creates a copy of this gameobject, without colliders or scripts, in each adjacent clone of the world.
    // Generally is not necessary unless a sprite is large enough that it could appear on both the top and bottom of a player's
    // screen as they walk around the planet.
    void CreateCopies() 
    {
        GameObject referenceToCopy = null;
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x != 0 || y != 0) {
                    // This is a pretty bad method of copying the Component over. It would be nice to copy the component,
                    // but that isn't built-in
                    // GameObject newGameObject = new GameObject("subsprite");
                    // newGameObject.transform.parent = transform;
                    // newGameObject.transform.position = transform.position + new Vector3(x*planetWidth, y*planetHeight);
                    // newGameObject.transform.localScale = new Vector3(1,1,1);
                    // // newGameObject.transform.rotation = transform.rotation;
                    // newGameObject.AddComponent<SpriteRenderer>();
                    // SpriteRenderer sr = newGameObject.GetComponent<SpriteRenderer>();
                    // SpriteRenderer mySR = GetComponent<SpriteRenderer>();
                    // sr.sprite = mySR.sprite;
                    // sr.sortingOrder = mySR.sortingOrder;
                    GameObject newGameObject;
                    if (referenceToCopy) {
                        newGameObject = Instantiate(referenceToCopy, transform);
                    }
                    else {
                        newGameObject = Instantiate(gameObject, transform);
                        referenceToCopy = newGameObject;
                    }
                    newGameObject.transform.position = transform.position + new Vector3(x*planetWidth, y*planetHeight);
                    MonoBehaviour[] scripts = newGameObject.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour script in scripts) {
                        script.enabled = false;
                    }
                    Rigidbody2D[] rbs = newGameObject.GetComponents<Rigidbody2D>();
                    foreach (Rigidbody2D rb in rbs) {
                        Destroy(rb);
                    }
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
    void TransformSelf(float x, float y)
    {
        transform.position += new Vector3(x, y, 0);
    }
}
