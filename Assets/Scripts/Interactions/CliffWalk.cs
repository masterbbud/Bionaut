using UnityEngine;

public class CliffWalk : MonoBehaviour
{
    // Reference to the player or player's Rigidbody2D
    [SerializeField] private Rigidbody2D playerRb;

    // Passable direction (true for right, false for left)
    [SerializeField] private bool canPassFromRight = true;

    private Collider2D objectCollider;

    private void Awake()
    {
        // Get the Collider2D component attached to this object
        objectCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // Check if the player is approaching from the correct side
        if (playerRb != null)
        {
            if (canPassFromRight && playerRb.position.x > transform.position.x)
            {
                // Player is approaching from the right; disable collider to allow passing
                objectCollider.enabled = false;
            }
            else if (!canPassFromRight && playerRb.position.x < transform.position.x)
            {
                // Player is approaching from the left; disable collider to allow passing
                objectCollider.enabled = false;
            }
            else
            {
                // Otherwise, enable the collider to block the player
                objectCollider.enabled = true;
            }
        }
    }
}

