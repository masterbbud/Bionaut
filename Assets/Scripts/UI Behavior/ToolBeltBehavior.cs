using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolBeltBehavior : MonoBehaviour
{
    private UIDocument ToolBeltUI;

    private Button RifleButton;

    private void Awake() 
    {
        ToolBeltUI = GetComponent<UIDocument>();
        RifleButton = ToolBeltUI.rootVisualElement.Q<Button>("Rifle");
        RifleButton.clicked += OnRifleButtonClicked;
    }

    private void OnRifleButtonClicked()
    {
        Debug.Log("Function Called");
    }
}
