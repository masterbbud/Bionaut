using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // DONT use a maxDistance > planetWidth/2 or > planetHeight/2, since this will cause the bullet to never disappear
    [SerializeField]
    private float maxDistance;

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) > maxDistance) {
            Destroy(gameObject);
        }
    }
}
