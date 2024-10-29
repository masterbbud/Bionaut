using UnityEngine;
public class TransparentObject : MonoBehaviour
{
    // Constants for fully visible and partially transparent alpha values
    const float VISIBLE_ALPHA = 1f;
    const float TRANSPARENT_ALPHA = 0.7f;

    // Array to hold all SpriteRenderer components of this object and its children
    private SpriteRenderer[] m_SpriteRenderers;
    // Flag to indicate if fading out is currently active
    private bool m_FadeOutEnabled = false;

    // Reference to the interacting player
    private Player m_Interactor;
    // Original sorting order of the interacting player
    private int m_InitialSortOrder;

    // Shortcut to access the main background object's SpriteRenderer
    private SpriteRenderer BackgroundObject => m_SpriteRenderers[0];
    // Shortcut to access the alpha value of the main background object
    private float BackgroundObjectAlpha => BackgroundObject.color.a;

    void Start()
    {
        // Gets all SpriteRenderer components in this object and its children
        m_SpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // If fading out is enabled and the background alpha is above the transparent threshold, continue fading out
        if (m_FadeOutEnabled && BackgroundObjectAlpha > TRANSPARENT_ALPHA)
        {
            FadeOut();

            // Once the background alpha reaches the transparent threshold, adjust the interactor's sorting order
            if (BackgroundObjectAlpha == TRANSPARENT_ALPHA)
            {
                m_Interactor.SpriteRenderer.sortingOrder = BackgroundObject.sortingOrder - 1;
            }
        }
        // If fading out is disabled and the background alpha is below the visible threshold, continue fading in
        else if (!m_FadeOutEnabled && BackgroundObjectAlpha < VISIBLE_ALPHA)
        {
            FadeIn();
            // Restore the interactor's initial sorting order when fully visible again
            m_Interactor.SpriteRenderer.sortingOrder = m_InitialSortOrder;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If already fading out, exit early
        if (m_FadeOutEnabled) return;

        // Check if the colliding object has a Player component
        if (collision.TryGetComponent<Player>(out Player player))
        {
            // Set the interactor to the colliding player and store their initial sorting order
            m_Interactor = player;
            m_InitialSortOrder = player.SpriteRenderer.sortingOrder;
            // Enable fade out effect
            m_FadeOutEnabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // If not fading out, exit early
        if (!m_FadeOutEnabled) return;

        // Check if the exiting object has a Player component
        if (collision.TryGetComponent<Player>(out Player player))
        {
            // Disable fade out effect when the player exits
            m_FadeOutEnabled = false;
        }
    }

    private void FadeOut()
    {
        // Set each SpriteRenderer in the array to a partially transparent alpha value
        foreach (SpriteRenderer renderer in m_SpriteRenderers)
        {
            ChangeOpacity(renderer, TRANSPARENT_ALPHA);
        }
    }

    private void FadeIn()
    {
        // Set each SpriteRenderer in the array to fully visible alpha value
        foreach (SpriteRenderer renderer in m_SpriteRenderers)
        {
            ChangeOpacity(renderer, VISIBLE_ALPHA);
        }
    }

    private void ChangeOpacity(SpriteRenderer renderer, float targetAlpha)
    {
        // Smoothly change the alpha of the given renderer towards the target alpha value
        Color color = renderer.color;
        Color smoothColor = new Color(color.r, color.g, color.b,
            Mathf.MoveTowards(color.a, targetAlpha, Time.deltaTime * 2)
        );
        renderer.color = smoothColor;
    }
}
