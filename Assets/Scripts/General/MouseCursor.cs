using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Static script for controlling the custom mouse cursor.
 * Follows the mouse position, but in world space.
 */
public class MouseCursor : MonoBehaviour
{
    private static Vector2 position;
    private static bool initialized = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Never have multiple mouse cursors on the screen
        if (initialized) {
            DestroyImmediate(gameObject);
            return;
        }

        // This is the first time this has loaded, so make it stay between scenes
        DontDestroyOnLoad(gameObject);
        initialized = true;
    }

    // Keeps this in the same position as the mouse cursor
    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        position = transform.position;
    }

    // Use this function to get the current mouse position in Vector2 space
    public static Vector2 GetPosition()
    {
        return position;
    }
}
