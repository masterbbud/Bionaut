using System.Collections;
using UnityEngine;

public class AggressiveCritter : Critter
{
    [SerializeField]
    private float wanderTime, wanderRadius; // Wander parameters
    [SerializeField]
    private float wanderWeight, seekWeight, pathWeight; // Behavior weights
    [SerializeField]
    private float distance; // Distance to start attacking the player

    protected override void StartSubclass()
    {
        base.StartSubclass();
        animator = GetComponent<Animator>(); // Reference Animator
    }

    protected override Vector2 CalculateSteeringForces()
    {
        Vector2 wanderForce = Wander(wanderTime, wanderRadius) * wanderWeight;
        Vector2 seekForce = Seek(Player.main.transform.position) * seekWeight;

        Vector2 pathForce = Vector2.zero;

        if (Vector2.Distance(transform.position, Player.main.transform.position) < playerRadius)
        {
            pathForce = SeekOnPath() * pathWeight;
        }

        // Flip sprite based on direction
        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            spriteRenderer.flipX = rb.velocity.x < 0;
        }

        // Attack if within range
        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            animator.SetBool("isAttacking", true); // Start attack animation
            maxSpeed = 5;
            return seekForce;
        }

        // Reset attack animation
        animator.SetBool("isAttacking", false);
        maxSpeed = 3;

        return wanderForce + pathForce;
    }

    protected override Vector2 CalculateBehavior()
    {
        AttackCritterHit(); // Call attack logic
        return StopAttackPoint();
    }

    public override void OnKnifeHit()
    {
        base.OnKnifeHit();
        if (!knockedOut)
        {
            animator.SetBool("isHit", true); // Trigger hit animation
            StartCoroutine(ResetHitAnimation());
        }
    }

    public override void OnRifleHit()
    {
        base.OnRifleHit();
        animator.SetBool("isHit", true); // Trigger hit animation
        StartCoroutine(ResetHitAnimation());
    }

    private IEnumerator ResetHitAnimation()
    {
        yield return new WaitForSeconds(1.0f); // Adjust duration to match animation
        animator.SetBool("isHit", false);
    }

    protected override void PlaySound()
    {
        FindObjectOfType<AudioManager>().Play("RattleSound");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Vector2.Distance(Player.main.transform.position, transform.position) < distance)
        {
            Gizmos.DrawLine(Player.main.transform.position, transform.position);
        }
    }
}
