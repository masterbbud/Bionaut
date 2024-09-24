using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPlanet : MonoBehaviour
{
    private bool expanded = false;
    private bool fullyExpanded = false;
    private float expandLerpValue = 0f;
    private static readonly float zoomSeconds = 1;
    
    public string planetName;
    public string description;
    public string sceneName;

    public GameObject planetOverlay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (expanded == true && expandLerpValue < 1) {
            //expandLerpValue += Time.deltaTime * 
        }
        if (fullyExpanded && Input.GetMouseButtonDown(0) && !GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition))) 
        {
            // leave planet selection
            StartCoroutine(ZoomOut());
        }
    }

    IEnumerator ZoomOut()
    {
        planetOverlay.SetActive(false);
        fullyExpanded = false;
        float planetSize = GetComponent<SpriteRenderer>().bounds.extents.y * 2;
        for (float zoom = 0f; zoom <= 1f; zoom += .01f / zoomSeconds)
        {
            Camera.main.transform.position = new Vector3(
                Mathf.SmoothStep(transform.position.x - planetSize * 0.6f, 0, zoom),
                Mathf.SmoothStep(transform.position.y, 0, zoom),
                -10
            );
            Camera.main.orthographicSize = Mathf.SmoothStep(planetSize * 0.75f, 5, zoom);
            yield return new WaitForSeconds(.01f);
        }
        expanded = false;
    }

    IEnumerator ZoomIn()
    {
        expanded = true;
        float planetSize = GetComponent<SpriteRenderer>().bounds.extents.y * 2;
        Debug.Log(planetSize);
        for (float zoom = 0f; zoom <= 1f; zoom += .01f / zoomSeconds)
        {
            Camera.main.transform.position = new Vector3(
                Mathf.SmoothStep(0, transform.position.x - planetSize * 0.6f, zoom),
                Mathf.SmoothStep(0, transform.position.y, zoom),
                -10
            );
            Camera.main.orthographicSize = Mathf.SmoothStep(5, planetSize * 0.75f, zoom);
            yield return new WaitForSeconds(.01f);
        }
        fullyExpanded = true;
        
        // show planet description
        planetOverlay.transform.Find("Name").GetComponent<TMP_Text>().text = planetName;
        planetOverlay.transform.Find("Description").GetComponent<TMP_Text>().text = description;
        // planetOverlay.transform.Find("LaunchButton").GetComponent<Button>().onClick = description;
        planetOverlay.SetActive(true);

    }

    void GoToThisPlanet() {
        Debug.Log(sceneName);
    }

    void OnMouseDown()
    {
        if (!expanded) {
            expanded = true;
            StartCoroutine(ZoomIn());
        }
        else {
            // go to planet!
            SceneManager.LoadScene("InPodScene");
        }
    }
}
