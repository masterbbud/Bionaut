using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Script for controlling the planet in outer space, on the planet map screen
 */
public class UIPlanet : MonoBehaviour
{
    private bool expanded = false;
    private bool fullyExpanded = false;
    private float expandLerpValue = 0f;
    private static readonly float zoomSeconds = 1;

    public PlanetData planetData; // Stores data about the planet

    public GameObject planetOverlay;

    // Static variable to keep track of the selected planet
    public static string selectedPlanetScene = "Silva"; // Default to the Tutorial Planet

    void Update()
    {
        if (expanded == true && expandLerpValue < 1)
        {
            // Smooth expansion (currently not implemented)
            // expandLerpValue += Time.deltaTime * expandSpeed;
        }

        if (fullyExpanded && Input.GetMouseButtonDown(0) &&
            !GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            // Leave planet selection if clicked outside the planet
            StartCoroutine(ZoomOut());
        }
    }

    IEnumerator ZoomOut()
    {
        // Zoom out to see the entire galaxy over zoomSeconds
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
        // Zoom in to see the specific planet over zoomSeconds
        expanded = true;
        float planetSize = GetComponent<SpriteRenderer>().bounds.extents.y * 2;

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

        // Display planet description
        planetOverlay.transform.Find("Name").GetComponent<TMP_Text>().text = planetData.planetName;
        planetOverlay.transform.Find("Description").GetComponent<TMP_Text>().text = planetData.description;

        // Activate the overlay
        planetOverlay.SetActive(true);
    }

    void GoToThisPlanet()
    {
        // Assign the selected planet's scene name
        selectedPlanetScene = planetData.sceneName;
        Debug.Log($"Selected Planet: {selectedPlanetScene}");

        // Load the pod scene
        SceneManager.LoadScene("InPodScene");
    }

    void OnMouseDown()
    {
        if (!expanded)
        {
            expanded = true;
            StartCoroutine(ZoomIn());
        }
        else
        {
            // Go to the selected planet
            GoToThisPlanet();
        }
    }
}
