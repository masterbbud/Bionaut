using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Tool
{
    [SerializeField]
    private Collider2D coll;
    public override void Use()
    {
        // TODO Trigger animation
        // Something like animator.SetBool("UsingNet", true);

        transform.localPosition = Player.main.GetComponent<Player>().facingDirection * 0.25f;
        transform.localRotation = Quaternion.Euler(0, 0, (float)Math.Atan2(transform.localPosition.y, transform.localPosition.x) * Mathf.Rad2Deg - 90);

        // Turn off the hitbox in a few frames
        StartCoroutine(DisableHitbox());
    }

    IEnumerator DisableHitbox()
    {
        // If we don't want the net hitbox to start until a frame or two in, we can move this around

        // Enable the collider for a few frames
        coll.enabled = true;
        
        Player.main.GetComponent<Player>().AnimateUseKnife(true);

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // Disable the collider
        coll.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the object can be hit with a net, do so now
        IKnifeHittable hittable = other.GetComponent<IKnifeHittable>();
        if (hittable != null) {
            hittable.OnKnifeHit();

            // Only disable the collider if the object was net-hittable, this way exactly one object can be hit per swing
            coll.enabled = false;
        }
    }
}
