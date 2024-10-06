using System;
using System.Collections;
using UnityEngine;

public class Net : Tool
{
    [SerializeField]
    private Collider2D coll;
    [SerializeField]
    private SpriteRenderer sr;
    public override void Use()
    {
        // TODO Trigger animation
        // Something like animator.SetBool("UsingNet", true);

        transform.localPosition = Player.main.GetComponent<Player>().facingDirection * 0.75f;
        transform.localRotation = Quaternion.Euler(0, 0, (float)Math.Atan2(transform.localPosition.y, transform.localPosition.x) * Mathf.Rad2Deg - 90);

        // Turn off the hitbox in a few frames
        StartCoroutine(DisableHitbox());
    }

    IEnumerator DisableHitbox()
    {
        // If we don't want the net hitbox to start until a frame or two in, we can move this around

        // Enable the collider for a few frames
        coll.enabled = true;
        sr.enabled = true; // This will get removed eventually
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // Disable the collider
        coll.enabled = false;
        sr.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the object can be hit with a net, do so now
        INetHittable hittable = other.GetComponent<INetHittable>();
        if (hittable != null) {
            hittable.OnNetHit();

            // Only disable the collider if the object was net-hittable, this way exactly one object can be hit per swing
            coll.enabled = false;
        }
    }
}
