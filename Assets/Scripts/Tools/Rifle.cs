using UnityEngine;

public class Rifle : Tool
{
    public GameObject bulletFab;
    public float bulletSpeed;
    public override void Use()
    {
        GameObject fab = Instantiate(bulletFab);
        fab.transform.position = transform.position;
        Vector2 towards = (MouseCursor.GetPosition() - (Vector2)transform.position).normalized;
        fab.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(towards.y, towards.x) * Mathf.Rad2Deg);
        fab.GetComponent<Rigidbody2D>().velocity = towards * bulletSpeed;
    }
}
