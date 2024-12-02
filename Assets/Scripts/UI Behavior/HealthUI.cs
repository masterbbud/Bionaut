using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private float maxHealth = 7;

    //Saving reference to health as there is none in player
    private float health = 5;

    // Start is called before the first frame update
    void Awake()
    {
        //Grab values
        healthUI = GetComponent<UIDocument>();
        healthLabel = healthUI.rootVisualElement.Q<Label>("healthLabel");
        healthBar = healthUI.rootVisualElement.Q<ProgressBar>();

        //Set values
        healthLabel.text = "Health: " + NumAsPercent(health) + "%";
        healthBar.value = health;
    }

    float NumAsPercent(float num)
    {
        return Mathf.Round((num / maxHealth) * 100);
    }
}
