using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolBeltBehavior : MonoBehaviour
{
    private UIDocument toolBeltUI;
    private VisualElement menuPanel;

    private Button rifleButton;
    private Button emptyButton;
    private Player player;

    /// <summary>
    /// Grabs definitions from the UI document
    /// </summary>
    private void Awake()
    {
        toolBeltUI = GetComponent<UIDocument>();
        rifleButton = toolBeltUI.rootVisualElement.Q<Button>("btn-rifle");
        emptyButton = toolBeltUI.rootVisualElement.Q<Button>("btn-empty");
        menuPanel = toolBeltUI.rootVisualElement.Q<VisualElement>("panel");

        // Register callback events to button logic
        rifleButton.RegisterCallback<ClickEvent>(OnButtonClicked);
        emptyButton.RegisterCallback<ClickEvent>(OnButtonClicked);
    }

    /// <summary>
    /// Ensures that the UI is hidden on start
    /// </summary>
    private void Start()
    {
        menuPanel.style.display = DisplayStyle.None;
        player = Player.main.GetComponent<Player>();
    }

    private void Update()
    {
        // Ensures that the UI will be visible and interactible whenever space is held down
        if (Input.GetKeyDown(KeyCode.Space))
        {
            menuPanel.style.display = DisplayStyle.Flex;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // Releases UI once space is released
            menuPanel.style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// On click event for the rifle button, will eventually be a call on all buttons differing by class name once more tools are introduced
    /// </summary>
    private void OnButtonClicked(ClickEvent evt)
    {
        if (evt.target == emptyButton)
        {
            player.SelectTool(0);
        }
        else if (evt.target == rifleButton)
        {
            player.SelectTool(1);
        }
    }
}
