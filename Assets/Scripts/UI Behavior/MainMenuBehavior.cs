using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private Button menuButton;
    private Button closeButton;

    private Slider volume;

    private Button selectCritterButton;
    private VisualElement critterAnimation;
    private Label critterDescription;

    [SerializeField]
    private VisualTreeAsset critterTemplate;
    private VisualElement critterGridContiner;
    private CritterData selectedCritter;
    private int spriteIdx;

    //content pages
    private VisualElement controlsContent;
    private VisualElement crittersContent;
    private VisualElement toolsContent;
    private VisualElement optionsContent;

    public static bool showing = false;
    private static bool shouldShowMenu = false;

    private static bool initialized = false;

    //ensure that the UI does not destroy on load unless initialied, grab main document, and initalize buttons and callback functions
    private void Awake()
    {
        if (initialized)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            initialized = true;
        }
        mainMenuUI = GetComponent<UIDocument>();

        //call intialization and callback rendering functions
        InitializeContent();
        RegisterAllCallbacks();

    }

    //initializes all buttons and dynamically changed items and grabs them from the document
    private void InitializeContent()
    {
        controlsButton = mainMenuUI.rootVisualElement.Q<Button>("Controls");
        crittersButton = mainMenuUI.rootVisualElement.Q<Button>("Critters");
        toolsButton = mainMenuUI.rootVisualElement.Q<Button>("Tools");
        optionsButton = mainMenuUI.rootVisualElement.Q<Button>("Options");
        quitButton = mainMenuUI.rootVisualElement.Q<Button>("Quit");

        //volume = mainMenuUI.rootVisualElement.Q<Slider>("Volume");

        menuButton = mainMenuUI.rootVisualElement.Q<Button>("Menu");

        closeButton = mainMenuUI.rootVisualElement.Q<Button>("Close");
        selectCritterButton = mainMenuUI.rootVisualElement.Q<Button>("CritterSelectButton");
        critterDescription = mainMenuUI.rootVisualElement.Q<Label>("CritterDescription");
        critterGridContiner = mainMenuUI.rootVisualElement.Q<VisualElement>("CritterGridContainer");
        critterAnimation = mainMenuUI.rootVisualElement.Q<VisualElement>("CritterAnimation");

        controlsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("ControlsContent");
        crittersContent = mainMenuUI.rootVisualElement.Q<VisualElement>("CrittersContent");
        toolsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("ToolsContent");
        optionsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("OptionsContent");
    }

    //registers all button behavior
    private void RegisterAllCallbacks()
    {
        controlsButton.RegisterCallback<ClickEvent>(ButtonClicked);
        crittersButton.RegisterCallback<ClickEvent>(ButtonClicked);
        toolsButton.RegisterCallback<ClickEvent>(ButtonClicked);
        optionsButton.RegisterCallback<ClickEvent>(ButtonClicked);
        quitButton.RegisterCallback<ClickEvent>(ButtonClicked);
        closeButton.RegisterCallback<ClickEvent>(ButtonClicked);
        selectCritterButton.RegisterCallback<ClickEvent>(ChoosePlayerCritter);
        menuButton.RegisterCallback<ClickEvent>(menuClicked);
        //volume.RegisterCallback<ChangeEvent<Vector2>>(volumeSlide);

    }

    public void Start()
    {
        mainMenuUI.rootVisualElement.style.display = DisplayStyle.None;
        critterAnimation.schedule.Execute(SwapAnimationTexture).Every(animationSpeedMs);
    }

    //handle events with opening the menu and using critters menu page
    private void Update()
    {
        //turn off and on the mainUI
        if (
            (Input.GetKeyDown(KeyCode.E) || shouldShowMenu)
            && !showing
            && !CatchCritterDialogBehavior.showing
        )
        {
            shouldShowMenu = false;
            mainMenuUI.rootVisualElement.style.display = DisplayStyle.Flex;
            showing = true;
            Time.timeScale = 0;

            // Handle critters page logic
            if (crittersContent.style.display == DisplayStyle.Flex)
            {
                OnOpenCrittersPage();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) && showing)
        {
            CloseUI();
        }
    }

    private void CloseUI()
    {
        mainMenuUI.rootVisualElement.style.display = DisplayStyle.None;
        showing = false;
        Time.timeScale = 1;
    }

    public static void ShowMenu()
    {
        shouldShowMenu = true;
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
            OnOpenCrittersPage();
        }
        else if (evt.target == toolsButton)
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
        else if (evt.target == closeButton) //quits game
        {
            CloseUI();
        }
    }
    private void volumeSlide(ChangeEvent<Vector2> evt)
    {
        AudioManager.volumeControl = (int)evt.newValue.x; 
    }

    private void menuClicked(ClickEvent evt)
    {
        if (evt.target == menuButton)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }


    private void OnOpenCrittersPage()
    {
        ResetCritters();

        if (SceneManager.GetActiveScene().name == "InPodScene")
        {
            selectCritterButton.SetEnabled(true);
        }
        else
        {
            selectCritterButton.SetEnabled(false);
        }
    }

    private void SelectCritter(ClickEvent evt)
    {
        // Select critter based on currently chosen critter
        CritterData c = Player.inventory.GetCritter(((VisualElement)evt.target).name);
        selectCritterButton.style.display = DisplayStyle.Flex;
        if (c != null) {
            critterDescription.text = c.description;
            critterAnimation.style.backgroundImage = new StyleBackground(c.sprites[0].texture);
            critterAnimation.style.backgroundColor = c.planetColor;
            spriteIdx = 0;
            selectedCritter = c;
        }
    }

    private void ChoosePlayerCritter(ClickEvent evt)
    {
        if (Companion.main)
        {
            DestroyImmediate(Companion.main);
        }
        Instantiate(selectedCritter.prefab);
    }

    private void SwapAnimationTexture()
    {
        if (!selectedCritter)
        {
            return;
        }
        spriteIdx = (spriteIdx + 1) % selectedCritter.sprites.Count;
        critterAnimation.style.backgroundImage = new StyleBackground(
            selectedCritter.sprites[spriteIdx].texture
        );
    }

    private void ResetCritters()
    {
        spriteIdx = 0;
        foreach (VisualElement s in critterGridContiner.Children().ToList())
        {
            critterGridContiner.Remove(s);
        }
        Debug.Log("reset");
        foreach (CritterData c in Player.inventory.collectedCritters) {
            VisualElement newCritter = critterTemplate.Instantiate();
            critterGridContiner.Add(newCritter);
            newCritter.Q<VisualElement>("Background").style.backgroundImage = new StyleBackground(
                c.sprites[0].texture
            );
            newCritter.Q<VisualElement>("Background").RegisterCallback<ClickEvent>(SelectCritter);
            newCritter.Q<VisualElement>("Background").name = c.name;
        }
    }
}
