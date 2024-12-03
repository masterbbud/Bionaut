using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Contains the UI for the Player's toolbelt.
 */
public class ToolBeltBehavior : MonoBehaviour
{
    private UIDocument toolBeltUI;
    private VisualElement menuPanel;

    private Button knifeButton;
    private Button netButton;
    private Button rifleButton;
    private Button emptyButton;
    private Player player;

    //Visual elements for all the tools
    private VisualElement knifeImage;
    private VisualElement netImage;
    private VisualElement rifleImage;

    //private ButtonCreation radialButton = new ButtonCreation();

    // If true, the UI is being shown. This allows us to stop other events when the
    // tool selection UI is up
    public static bool showing;

    /// <summary>
    /// Grabs definitions from the UI document
    /// </summary>
    private void Awake()
    {
        toolBeltUI = GetComponent<UIDocument>();
        knifeImage = toolBeltUI.rootVisualElement.Q<VisualElement>("knife-image");
        netImage = toolBeltUI.rootVisualElement.Q<VisualElement>("net-image");
        rifleImage = toolBeltUI.rootVisualElement.Q<VisualElement>("rifle-image");

        netButton = toolBeltUI.rootVisualElement.Q<Button>("btn-net");
        rifleButton = toolBeltUI.rootVisualElement.Q<Button>("btn-rifle");
        emptyButton = toolBeltUI.rootVisualElement.Q<Button>("btn-empty");
        knifeButton = toolBeltUI.rootVisualElement.Q<Button>("btn-knife");
        menuPanel = toolBeltUI.rootVisualElement.Q<VisualElement>("panel");

        // menuPanel.Add(radialButton);

        // Register callback events to button logic
        netButton.RegisterCallback<ClickEvent>(OnButtonClicked);
        rifleButton.RegisterCallback<ClickEvent>(OnButtonClicked);
        emptyButton.RegisterCallback<ClickEvent>(OnButtonClicked);
        knifeButton.RegisterCallback<ClickEvent>(OnButtonClicked);
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
        if (Input.GetKeyDown(KeyCode.Space) && !CatchCritterDialogBehavior.showing)
        {
            menuPanel.style.display = DisplayStyle.Flex;
            showing = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // Releases UI once space is released
            menuPanel.style.display = DisplayStyle.None;
            showing = false;
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
            knifeImage.style.display = DisplayStyle.None;
            netImage.style.display = DisplayStyle.None;
            rifleImage.style.display = DisplayStyle.None;
        }
        else if (evt.target == rifleButton)
        {
            player.SelectTool(1);
            knifeImage.style.display = DisplayStyle.None;
            netImage.style.display = DisplayStyle.None;
            rifleImage.style.display = DisplayStyle.Flex;
        }
        else if (evt.target == netButton)
        {
            player.SelectTool(2);
            knifeImage.style.display = DisplayStyle.None;
            netImage.style.display = DisplayStyle.Flex;
            rifleImage.style.display = DisplayStyle.None;
        }
        else if (evt.target == knifeButton)
        {
            player.SelectTool(3);
            knifeImage.style.display = DisplayStyle.Flex;
            netImage.style.display = DisplayStyle.None;
            rifleImage.style.display = DisplayStyle.None;
        }
        evt.StopPropagation();
    }
}
