using UnityEngine;

public class Rifle : Tool
{
    public GameObject bulletFab;
    public float bulletSpeed;
    public override void Use()
    {
        // Spawn a bullet projectile
        GameObject fab = Instantiate(bulletFab);
        fab.transform.position = transform.position;
        
        // Figure out where the bullet should shoot towards
        Vector2 towards = (MouseCursor.GetPosition() - (Vector2)transform.position).normalized;
        fab.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(towards.y, towards.x) * Mathf.Rad2Deg);

        // Make it move
        fab.GetComponent<Rigidbody2D>().velocity = towards * bulletSpeed;
    }
}
