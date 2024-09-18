using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPlanet : MonoBehaviour
{
    private bool expanded = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (!expanded) {
            expanded = true;
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -4);

        }
        else {
            // go to planet!
            SceneManager.LoadScene("InPodScene");
        }
    }
}
