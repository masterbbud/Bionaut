using UnityEngine;
using UnityEngine.Tilemaps;

public class TransparentTilemap : MonoBehaviour
{
    const float VISIBLE_ALPHA = 1f;
    const float TRANSPARENT_ALPHA = 0.5f;

    // Reference to the Tilemap component
    private Tilemap m_Tilemap;
    private bool m_FadeOutEnabled = false;

    // Reference to the interacting player
    private Player m_Interactor;
    private int m_InitialSortOrder;

    void Start()
    {
        // Get the Tilemap component on this object
        m_Tilemap = GetComponent<Tilemap>();
    }

    void Update()
    {
        // If fading out is enabled and the tilemap alpha is above the transparent threshold, continue fading out
        if (m_FadeOutEnabled && m_Tilemap.color.a > TRANSPARENT_ALPHA)
        {
            FadeOut();

            // Adjust the interactor's sorting order when transparency threshold is reached
            if (m_Tilemap.color.a == TRANSPARENT_ALPHA)
            {
                m_Interactor.SpriteRenderer.sortingOrder = m_Tilemap.GetComponent<TilemapRenderer>().sortingOrder - 1;
            }
        }
        // If fading out is disabled and the tilemap alpha is below the visible threshold, continue fading in
        else if (!m_FadeOutEnabled && m_Tilemap.color.a < VISIBLE_ALPHA)
        {
            FadeIn();
            m_Interactor.SpriteRenderer.sortingOrder = m_InitialSortOrder;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_FadeOutEnabled) return;

        if (collision.TryGetComponent<Player>(out Player player))
        {
            m_Interactor = player;
            m_InitialSortOrder = player.SpriteRenderer.sortingOrder;
            m_FadeOutEnabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!m_FadeOutEnabled) return;

        if (collision.TryGetComponent<Player>(out Player player))
        {
            m_FadeOutEnabled = false;
        }
    }

    private void FadeOut()
    {
        // Adjust tilemap color alpha to be partially transparent
        ChangeOpacity(TRANSPARENT_ALPHA);
    }

    private void FadeIn()
    {
        // Restore tilemap color alpha to be fully visible
        ChangeOpacity(VISIBLE_ALPHA);
    }

    private void ChangeOpacity(float targetAlpha)
    {
        Color color = m_Tilemap.color;
        color.a = Mathf.MoveTowards(color.a, targetAlpha, Time.deltaTime * 2);
        m_Tilemap.color = color;
    }
}
