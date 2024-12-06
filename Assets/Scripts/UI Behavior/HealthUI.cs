using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HealthUI : MonoBehaviour
{
    //Reference to document
    private UIDocument healthUI;

    //Slider
    private ProgressBar healthBar;

    //Label for health
    private Label healthLabel;

    //Reference to player
    [SerializeField]
    private GameObject player;

    // Max health
    private float maxHealth = 100;

    //Saving reference to health as there is none in player
    private float health = 5;

    private bool mainShowing = false;

    // Start is called before the first frame update
    void Awake()
    {
        //Grab values
        healthUI = GetComponent<UIDocument>();
        healthLabel = healthUI.rootVisualElement.Q<Label>("healthLabel");
        healthBar = healthUI.rootVisualElement.Q<ProgressBar>();
        health = player.GetComponent<Player>().health;

        //Set values
        healthBar.value = health;
        healthLabel.text = "Health: " + NumAsPercent(health) + "%";

        SceneManager.sceneLoaded += SetMenuActiveByScene;
    }

    //Update
    void Update()
    {
        health = player.GetComponent<Player>().health;
        healthBar.value = health;
        healthLabel.text = "Health: " + NumAsPercent(health) + "%";

        HandleKeyPress();
    }

    float NumAsPercent(float num)
    {
        return Mathf.Round((num / maxHealth) * 100);
    }

    void HandleKeyPress()
    {
        //Goes away if pressed
        if (mainShowing)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                healthUI.rootVisualElement.style.display = DisplayStyle.Flex;
                mainShowing = false;
            }
        }
        else if (!mainShowing)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                healthUI.rootVisualElement.style.display = DisplayStyle.None;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                healthUI.rootVisualElement.style.display = DisplayStyle.Flex;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                healthUI.rootVisualElement.style.display = DisplayStyle.None;
                mainShowing = true;
            }
        }
    }

    void SetMenuActiveByScene(Scene scene, LoadSceneMode mode)
    {
        // Player should be inactive on the planet map scene and start scene
        if (scene.name == "PlanetMapScene" || scene.name == "MainMenu")
        {
            healthUI.rootVisualElement.style.display = DisplayStyle.None;
        }
    }
}
