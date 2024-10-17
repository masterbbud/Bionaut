using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuBehavior : MonoBehaviour
{
    private UIDocument mainMenuUI;

    //buttons
    private Button controlsButton;
    private Button crittersButton;
    private Button toolsButton;
    private Button optionsButton;
    private Button quitButton;

    //content
    private VisualElement controlsContent;
    private VisualElement crittersContent;
    private VisualElement toolsContent;
    private VisualElement optionsContent;

    public static bool showing = false;

    //a ton of query selectors, and registering callback function
    private void Awake()
    {
        mainMenuUI = GetComponent<UIDocument>();
        controlsButton = mainMenuUI.rootVisualElement.Q<Button>("Controls");
        crittersButton = mainMenuUI.rootVisualElement.Q<Button>("Critters");
        toolsButton = mainMenuUI.rootVisualElement.Q<Button>("Tools");
        optionsButton = mainMenuUI.rootVisualElement.Q<Button>("Options");
        quitButton = mainMenuUI.rootVisualElement.Q<Button>("Quit");

        controlsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("ControlsContent");
        crittersContent = mainMenuUI.rootVisualElement.Q<VisualElement>("CrittersContent");
        toolsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("ToolsContent");
        optionsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("OptionsContent");

        controlsButton.RegisterCallback<ClickEvent>(ButtonClicked);
        crittersButton.RegisterCallback<ClickEvent>(ButtonClicked);
        toolsButton.RegisterCallback<ClickEvent>(ButtonClicked);
        optionsButton.RegisterCallback<ClickEvent>(ButtonClicked);
        quitButton.RegisterCallback<ClickEvent>(ButtonClicked);
    }

    public void Start()
    {
        mainMenuUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        //turn off and on the mainUI
        if (Input.GetKeyDown(KeyCode.E) && !showing)
        {
            mainMenuUI.rootVisualElement.style.display = DisplayStyle.Flex;
            showing = true;
        }
        else if(Input.GetKeyDown(KeyCode.E) && showing)
        {
            mainMenuUI.rootVisualElement.style.display= DisplayStyle.None;
            showing = false;
        }
    }

    /// <summary>
    /// handle button events
    /// </summary>
    /// <param name="evt">button clicked</param>
    private void ButtonClicked(ClickEvent evt)
    {
        //changes display for each panel to show only the correct one
        if (evt.target == controlsButton)
        {
            controlsContent.style.display = DisplayStyle.Flex;
            crittersContent.style.display = DisplayStyle.None;
            toolsContent.style.display = DisplayStyle.None;
            optionsContent.style.display = DisplayStyle.None;
        }
        else if (evt.target == crittersButton)
        {
            controlsContent.style.display = DisplayStyle.None;
            crittersContent.style.display = DisplayStyle.Flex;
            toolsContent.style.display = DisplayStyle.None;
            optionsContent.style.display = DisplayStyle.None;
        }
        else if ( evt.target == toolsButton)
        {
            controlsContent.style.display = DisplayStyle.None;
            crittersContent.style.display = DisplayStyle.None;
            toolsContent.style.display = DisplayStyle.Flex;
            optionsContent.style.display = DisplayStyle.None;
        }
        else if (evt.target == optionsButton)
        {
            controlsContent.style.display = DisplayStyle.None;
            crittersContent.style.display = DisplayStyle.None;
            toolsContent.style.display = DisplayStyle.None;
            optionsContent.style.display = DisplayStyle.Flex;
        }
        else if (evt.target == quitButton) //quits game
        {
            Application.Quit();
            Debug.Log("Game Quit");
        }
    }
}
