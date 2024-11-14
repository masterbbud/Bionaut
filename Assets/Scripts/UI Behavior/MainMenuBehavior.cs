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
    private Button itemsButton;
    private Button crittersButton;
    private Button toolsButton;
    private Button optionsButton;
    private Button quitButton;
    private Button menuButton;

    private Slider volume;
    private Button closeButton;

    private Button selectCritterButton;
    private VisualElement critterAnimation;
    private Label critterName;
    private Label critterDescription;
    [SerializeField]
    private VisualTreeAsset critterTemplate;
    private VisualElement critterGridContiner;
    private CritterData selectedCritter;
    private int spriteIdx;
    [SerializeField] private int animationSpeedMs;


    private VisualElement itemSprite;
    private Label itemCount;
    private Label itemName;
    private Label itemDescription;
    [SerializeField]
    private VisualTreeAsset itemTemplate;
    private VisualElement itemGridContainer;

    //content
    private VisualElement controlsContent;
    private VisualElement itemsContent;
    private VisualElement crittersContent;
    private VisualElement toolsContent;
    private VisualElement optionsContent;

    public static bool showing = false;
    private static bool shouldShowMenu = false;

    private static bool initialized = false;

    //a ton of query selectors, and registering callback function
    private void Awake()
    {
        if (initialized) {
            DestroyImmediate(gameObject);
            return;
        } else {
            DontDestroyOnLoad(gameObject);
            initialized = true;
        }

        InitializeContent();
        RegisterAllCallbacks();
    }

    private void InitializeContent() {
        mainMenuUI = GetComponent<UIDocument>();
        controlsButton = mainMenuUI.rootVisualElement.Q<Button>("Controls");
        itemsButton = mainMenuUI.rootVisualElement.Q<Button>("Inventory");
        crittersButton = mainMenuUI.rootVisualElement.Q<Button>("Critters");
        toolsButton = mainMenuUI.rootVisualElement.Q<Button>("Tools");
        optionsButton = mainMenuUI.rootVisualElement.Q<Button>("Options");
        quitButton = mainMenuUI.rootVisualElement.Q<Button>("Quit");

        //volume = mainMenuUI.rootVisualElement.Q<Slider>("Volume");
        menuButton = mainMenuUI.rootVisualElement.Q<Button>("Menu");
        closeButton = mainMenuUI.rootVisualElement.Q<Button>("Close");

        selectCritterButton = mainMenuUI.rootVisualElement.Q<Button>("CritterSelectButton");
        critterName = mainMenuUI.rootVisualElement.Q<Label>("CritterName");
        critterDescription = mainMenuUI.rootVisualElement.Q<Label>("CritterDescription");
        critterGridContiner = mainMenuUI.rootVisualElement.Q<VisualElement>("CritterGridContainer");
        critterAnimation = mainMenuUI.rootVisualElement.Q<VisualElement>("CritterAnimation");

        itemName = mainMenuUI.rootVisualElement.Q<Label>("ItemName");
        itemCount = mainMenuUI.rootVisualElement.Q<Label>("ItemCount");
        itemDescription = mainMenuUI.rootVisualElement.Q<Label>("ItemDescription");
        itemGridContainer = mainMenuUI.rootVisualElement.Q<VisualElement>("InventoryGridContainer");
        itemSprite = mainMenuUI.rootVisualElement.Q<VisualElement>("ItemSprite");

        controlsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("ControlsContent");
        itemsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("InventoryContent");
        crittersContent = mainMenuUI.rootVisualElement.Q<VisualElement>("CrittersContent");
        toolsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("ToolsContent");
        optionsContent = mainMenuUI.rootVisualElement.Q<VisualElement>("OptionsContent");
    }

    private void RegisterAllCallbacks() {
        controlsButton.RegisterCallback<ClickEvent>(ButtonClicked);
        itemsButton.RegisterCallback<ClickEvent>(ButtonClicked);
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

    private void Update()
    {
        //turn off and on the mainUI
        if ((Input.GetKeyDown(KeyCode.E) || shouldShowMenu) && !showing && !CatchCritterDialogBehavior.showing)
        {
            shouldShowMenu = false;
            mainMenuUI.rootVisualElement.style.display = DisplayStyle.Flex;
            showing = true;
            Time.timeScale = 0;

            // Handle critters page logic
            if (crittersContent.style.display == DisplayStyle.Flex) {
                OnOpenCrittersPage();
            }
            if (itemsContent.style.display == DisplayStyle.Flex) {
                ResetItems();
            }
        }
        else if(Input.GetKeyDown(KeyCode.E) && showing)
        {
            CloseUI();
        }
    }

    private void CloseUI()
    {
        mainMenuUI.rootVisualElement.style.display= DisplayStyle.None;
        showing = false;
        Time.timeScale = 1;
    }

    public static void ShowMenu() {
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
            itemsContent.style.display = DisplayStyle.None;
            crittersContent.style.display = DisplayStyle.None;
            toolsContent.style.display = DisplayStyle.None;
            optionsContent.style.display = DisplayStyle.None;
        }
        else if (evt.target == itemsButton)
        {
            controlsContent.style.display = DisplayStyle.None;
            itemsContent.style.display = DisplayStyle.Flex;
            crittersContent.style.display = DisplayStyle.None;
            toolsContent.style.display = DisplayStyle.None;
            optionsContent.style.display = DisplayStyle.None;
            ResetItems();
        }
        else if (evt.target == crittersButton)
        {
            controlsContent.style.display = DisplayStyle.None;
            itemsContent.style.display = DisplayStyle.None;
            crittersContent.style.display = DisplayStyle.Flex;
            toolsContent.style.display = DisplayStyle.None;
            optionsContent.style.display = DisplayStyle.None;
            OnOpenCrittersPage();
        }
        else if ( evt.target == toolsButton)
        {
            controlsContent.style.display = DisplayStyle.None;
            itemsContent.style.display = DisplayStyle.None;
            crittersContent.style.display = DisplayStyle.None;
            toolsContent.style.display = DisplayStyle.Flex;
            optionsContent.style.display = DisplayStyle.None;
        }
        else if (evt.target == optionsButton)
        {
            controlsContent.style.display = DisplayStyle.None;
            itemsContent.style.display = DisplayStyle.None;
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

    private void OnOpenCrittersPage() {
        ResetCritters();
        
        if (SceneManager.GetActiveScene().name == "InPodScene") {
            selectCritterButton.SetEnabled(true);
        } else {
            selectCritterButton.SetEnabled(false);
        }
    }

    private void SelectCritter(ClickEvent evt) {
        // Select critter based on currently chosen critter
        VisualElement evtTarget = (VisualElement)evt.target;
        CritterData c = Player.inventory.GetCritter(evtTarget.name);
        selectCritterButton.style.display = DisplayStyle.Flex;
        if (c != null) {
            critterName.text = Player.inventory.GetCritterName(c);
            critterDescription.text = c.description;
            critterAnimation.style.backgroundImage = new StyleBackground(c.sprites[0].texture);
            critterAnimation.style.backgroundColor = c.planetColor;
            spriteIdx = 0;
            selectedCritter = c;
        }
    }

    private void ChoosePlayerCritter(ClickEvent evt) {
        if (Companion.main) {
            DestroyImmediate(Companion.main);
        }
        Instantiate(selectedCritter.prefab);
    }

    private void SwapAnimationTexture() {
        if (!selectedCritter) { return; }
        spriteIdx = (spriteIdx + 1) % selectedCritter.sprites.Count;
        critterAnimation.style.backgroundImage = new StyleBackground(selectedCritter.sprites[spriteIdx].texture);
    }

    private void ResetCritters() {
        spriteIdx = 0;
        foreach (VisualElement s in critterGridContiner.Children().ToList()) {
            critterGridContiner.Remove(s);
        }
        foreach (CritterData c in Player.inventory.collectedCritters) {
            VisualElement newCritter = critterTemplate.Instantiate();
            critterGridContiner.Add(newCritter);
            newCritter.Q<VisualElement>("Background").style.backgroundImage = new StyleBackground(c.sprites[0].texture);
            newCritter.Q<VisualElement>("Background").RegisterCallback<ClickEvent>(SelectCritter);
            newCritter.Q<VisualElement>("Background").name = c.name;
        }
        
    }

    private void SelectItem(ClickEvent evt) {
        // Select critter based on currently chosen critter
        VisualElement evtTarget = (VisualElement)evt.target;
        ItemData i = Player.inventory.GetItem(evtTarget.name);
        if (i != null) {
            itemName.text = i.itemName;
            itemCount.text = "x" + Player.inventory.collectedItems[i].ToString();
            itemDescription.text = i.description;
            itemSprite.style.backgroundImage = new StyleBackground(i.sprite);
        }
    }

    private void SetBorderWidth(VisualElement e, int width) {
        e.style.borderTopWidth = width;
        e.style.borderBottomWidth = width;
        e.style.borderLeftWidth = width;
        e.style.borderRightWidth = width;
    }

    private void ResetItems() {
        foreach (VisualElement s in itemGridContainer.Children().ToList()) {
            itemGridContainer.Remove(s);
        }
        foreach (ItemData i in Player.inventory.collectedItems.Keys) {
            VisualElement newItem = itemTemplate.Instantiate();
            itemGridContainer.Add(newItem);
            newItem.Q<VisualElement>("Background").style.backgroundImage = new StyleBackground(i.sprite);
            newItem.Q<VisualElement>("Background").RegisterCallback<ClickEvent>(SelectItem);
            newItem.Q<VisualElement>("Background").name = i.itemName;
            newItem.Q<Label>("Label").text = "x" + Player.inventory.collectedItems[i].ToString();
        }
        
    }
}
