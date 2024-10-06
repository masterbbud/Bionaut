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
        // Trigger animation
        // Something like animator.SetBool("UsingNet", true);

        // Probably will be some delay before this happens

        transform.localPosition = Player.main.GetComponent<Player>().facingDirection * 0.75f;
        transform.localRotation = Quaternion.Euler(0, 0, (float)Math.Atan2(transform.localPosition.y, transform.localPosition.x) * Mathf.Rad2Deg - 90);

        // Turn off the hitbox next frame
        StartCoroutine(DisableHitbox());
    }

    IEnumerator DisableHitbox()
    {
        // If we don't want the net hitbox to start until a frame or two in, we can move this around
        coll.enabled = true;
        sr.enabled = true; // This will get removed eventually
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        coll.enabled = false;
        sr.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        INetHittable hittable = other.GetComponent<INetHittable>();
        if (hittable != null) {
            hittable.OnNetHit();
            coll.enabled = false;
        }
    }
}
