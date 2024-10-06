using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MouseCursor : MonoBehaviour
{
    private static Vector2 position;
    private static bool initialized = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (initialized) {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        initialized = true;
    }

    // Update is called once per frame
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
